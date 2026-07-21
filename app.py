"""
SSE server for polling various PLCs & other IOT even REST interfaces

Uses Flask + streaming SSE

One polling thread per interface

Starts polling when first client connects

Stops polling when last client disconnects

Keeps last_result cache

SSE route:
  start_polling():
    first time:
      load driver
      connect to driver
    poll_loop():
      poll_interface():
        read driver
        send response to client
        pause for scan-interval-ms

"""

from flask import Flask, Response, request, jsonify
from flask_cors import CORS
import os
import json
import yaml
import threading
import time
import importlib
#import requests # For webport TODO - remove
from pathlib import Path
from datetime import datetime

APP_ROOT = Path(__file__).parent
INTERFACES_DIR = APP_ROOT / "config" / "interfaces"
SYSTEMS_DIR = APP_ROOT / "config" / "systems"

PORT = 3000
POLL_FREQUENCY = 60000  # 60 seconds

app = Flask(__name__)
app.json.sort_keys = False

CORS(app, origins="*")

# ─────────────────────────────────────────────────────────────
# Globals
# ─────────────────────────────────────────────────────────────

interface_configs = {}      # { interface_name: config }
interface_drivers = {}      # { interface_name: driver }
interface_clients = {}      # { interface_name: [Queue, ...] }
interface_threads = {}      # { interface_name: Thread }
stop_flags = {}             # { interface_name: threading.Event }
last_result = {}            # { interface_name: data }

lock = threading.Lock()


# ─────────────────────────────────────────────────────────────
# Helpers
# ─────────────────────────────────────────────────────────────

def timestamp():
    return datetime.now().strftime("%Y-%m-%d %H:%M:%S")

def load_interface_config(interface_name):
    if interface_name in interface_configs:
        return interface_configs[interface_name]

    path = INTERFACES_DIR / f"{interface_name}.yaml"
    with open(path, "r", encoding="utf-8") as f:
        config = yaml.safe_load(f)

    if "name" not in config or "driver" not in config:
        raise ValueError(f"Missing required config fields in {interface_name}.yaml")

    interface_configs[interface_name] = config
    return config

def load_driver (config):

    module = importlib.import_module(
        f"sources.{config['driver'].replace('-', '_')}"
    )

    return module.Driver(config)
   
def poll_interface(interface_name):
    config = interface_configs.get(interface_name)
    if not config:
        raise ValueError(f"Unrecognised config {interface_name}")

    driver = interface_drivers[interface_name]
    return driver.read()


def poll_loop(interface_name, interval, stop_event):
    print(f"[{timestamp()}] Starting polling of interface {interface_name} every {interval}s")

    while not stop_event.is_set():
        try:
            print(f"[{timestamp()}] polling {interface_name}")
            data = {
                "interface": interface_name,
                "timestamp": timestamp(),
                "sources": poll_interface(interface_name),
            }
            with lock:
                last_result[interface_name] = data
                for q in interface_clients.get(interface_name, []):
                    q.append(data)
        except Exception as e:
            err = {"type": "error", "code": 500, "message": str(e)}
            with lock:
                for q in interface_clients.get(interface_name, []):
                    q.append(err)

        stop_event.wait(interval)

    print(f"[{timestamp()}] Stopped polling interface {interface_name}")


def start_polling(interface_name, config):
    if interface_name in interface_threads:
        return

    if interface_name not in interface_drivers:
        interface_drivers[interface_name] = load_driver(config)
        driver = interface_drivers[interface_name]
        driver.connect()

    try:
        interval = float(config.get("scan-interval-ms", POLL_FREQUENCY))/ 1000 # for milliseconds
    except ValueError:
        raise ValueError(f"Invalid poll-frequency for {interface_name}")

    stop_event = threading.Event()
    stop_flags[interface_name] = stop_event

    thread = threading.Thread(
        target=poll_loop,
        args=(interface_name, interval, stop_event),
        daemon=True,
    )
    interface_threads[interface_name] = thread
    thread.start()


def stop_polling(interface_name):
    stop_event = stop_flags.pop(interface_name, None)
    if stop_event:
        stop_event.set()

    interface_threads.pop(interface_name, None)
    interface_drivers.pop(interface_name, None) 
    last_result.pop(interface_name, None)


# ─────────────────────────────────────────────────────────────
# SSE route
# ─────────────────────────────────────────────────────────────

@app.route("/api/interfaces/<interface>/get-sources")
def sse_interface(interface):
    try:
        config = load_interface_config(interface)
        interface_name = config["name"]

        client_queue = []

        with lock:
            interface_clients.setdefault(interface_name, []).append(client_queue)

            if interface_name not in interface_threads:
                start_polling(interface_name, config)

            if interface_name in last_result:
                client_queue.append(last_result[interface_name])

        def stream():
            try:
                while True:
                    while client_queue:
                        data = client_queue.pop(0)
                        yield f"data: {json.dumps(data)}\n\n"
                    time.sleep(0.2)
            finally:
                with lock:
                    interface_clients[interface_name].remove(client_queue)
                    if not interface_clients[interface_name]:
                        stop_polling(interface_name)
                        del interface_clients[interface_name]

        return Response(stream(), mimetype="text/event-stream")

    except FileNotFoundError:
        return jsonify({"error": f"Interface {interface} not found"}), 404

    except Exception as e:
        print(f"[{timestamp()}] Error handling interface {interface}: {e}")
        return "", 500


# ─────────────────────────────────────────────────────────────
# REST endpoints
# ─────────────────────────────────────────────────────────────

@app.route("/api/interfaces")
def list_interfaces():
    try:
        return jsonify([
            f.stem for f in INTERFACES_DIR.iterdir()
            if f.suffix == ".yaml"
        ])
    except Exception as e:
        print(f"[{timestamp()}] Error listing interfaces: {e}")
        return "", 500


@app.route("/api/interfaces/<interface>")
def get_interface(interface):
    try:
        path = INTERFACES_DIR / f"{interface}.yaml"
        with open(path, "r", encoding="utf-8") as f:
            #y = json.dumps(yaml.safe_load(f))
            #print (y)
            return jsonify(yaml.safe_load(f))

    except FileNotFoundError:
        return jsonify({"error": "Not found"}), 404

    except Exception as e:
        print(f"[{timestamp()}] Error reading interface {interface}: {e}")
        return "", 500

@app.route("/api/interfaces/<interface>/write", methods=["POST"])
def write_source(interface):

    try:
        driver = interface_drivers[interface]

        body = request.json

        driver.write(
            body["source"],
            body["value"],
        )

        return "", 204

    except KeyError:
        return jsonify({"error": "Unknown interface"}), 404

    except PermissionError as e:
        return jsonify({"error": str(e)}), 403

    except Exception as e:
        print(f"[{timestamp()}] Write failed: {e}")
        return jsonify({"error": str(e)}), 500

@app.route("/api/systems/<system>")
def get_system(system):
    try:
        #path = APP_ROOT / "public" / "systems" / f"{system}.yaml"
        path = SYSTEMS_DIR / f"{system}.yaml"
        with open(path, "r", encoding="utf-8") as f:
            return jsonify(yaml.safe_load(f))

    except FileNotFoundError:
        return jsonify({"error": f"System {system} not found"}), 404

    except Exception as e:
        print(f"[{timestamp()}] Error reading system {system}: {e}")
        return "", 500


# ─────────────────────────────────────────────────────────────
# Web client
# ─────────────────────────────────────────────────────────────
from flask import send_from_directory

@app.route("/")
def index():
    return send_from_directory("static", "index.html")


@app.route("/<path:path>")
def static_proxy(path):
    return send_from_directory("static", path)

# ─────────────────────────────────────────────────────────────
# Main
# ─────────────────────────────────────────────────────────────

if __name__ == "__main__":
    print(f"[{timestamp()}] Server listening on port {PORT}")
    app.run(
      debug=True, # Hot reload
      host="0.0.0.0", # Needed for windows
      port=PORT, 
      threaded=True
    )
