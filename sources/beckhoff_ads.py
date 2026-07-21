import pyads


class Driver:

    def __init__(self, config):

        print("ADS Creating connection")
        self.plc = pyads.Connection(
            config["ams-net-id"],
            config.get("port", 851),
            config["ip-address"]
        )

    def connect(self):
        try:
            #self.plc.set_timeout (3000) does nothing if no PLC
            print("ADS Opening")
            self.plc.open()

            print(f"ADS Opened")
            self.sources = {
                source["id"]: source
                for source in config["sources"]
            }
        except Exception as e:
            raise ConnectionError (
                f"Unable to connect to {config['name']}: {e}"
            )

    def read(self):

        try:
            values = {}

            for source in self.sources.values():

                values[source["id"]] = self.plc.read_by_name(
                    source["symbol"]
                )

            return values

        except Exception as e:
            raise RuntimeError(
                f"Read failed: {e}"
            )

    def write(self, source, value):

        try:
            cfg = self.sources[source]

            if not cfg.get("writable", False):
                raise PermissionError(f"{source} is not writable")

            self.plc.write_by_name(
                cfg["symbol"],
                value,
            )
        except Exception as e:
            raise RuntimeError(
                f"Write failed: {e}"
            )

    def __del__(self):

        try:
            self.plc.close()
        except Exception:
            pass