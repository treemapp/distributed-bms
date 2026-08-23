"""
Connect to a Beckhoff PLC and generate an interface.yaml
from its exposed PLC symbols.

Usage:
    python create_interface.py \
        --ams-id 5.2.78.118.1.1 \
        --ip-address 192.168.0.208

    python create_interface.py \
        --ams-id 5.2.78.118.1.1 \
        --port 801 \
        --ip-address 192.168.0.208 \
        --output my-interface.yaml

Port defaults to 851 (TwinCAT 3).
"""

import argparse
import sys

import pyads
import yaml


def main():

    parser = argparse.ArgumentParser(
        description="Generate interface.yaml from a Beckhoff PLC."
    )

    parser.add_argument(
        "--ams-id",
        required=True,
        help="Beckhoff AMS Net ID, e.g. 5.2.78.118.1.1",
    )

    parser.add_argument(
        "--port",
        type=int,
        default=851,
        help="ADS port (default: 851)",
    )

    parser.add_argument(
        "--ip-address",
        required=True,
        help="IP address of the Beckhoff PLC",
    )

    parser.add_argument(
        "--output",
        default="interface.yaml",
        help="Output YAML file (default: interface.yaml)",
    )

    args = parser.parse_args()

    plc = pyads.Connection(
        args.ams_id,
        args.port,
        args.ip_address,
    )

    try:

        print(
            f"Connecting to {args.ip_address} "
            f"(AMS {args.ams_id}, port {args.port})..."
        )

        plc.open()

        print("Connected.")
        print("Reading symbols...")

        symbols = plc.get_all_symbols()

        sources = []

        for symbol in symbols:

            # Ignore TwinCAT internal symbols
            if symbol.name.startswith("."):
                continue

            sources.append({
                "id": symbol.name,
                "type": symbol.symbol_type,
            })

        config = {
            "version": 1,
            "name": "beckhoff",
            "driver": "beckhoff-ads",
            "ip-address": args.ip_address,
            "ams-net-id": args.ams_id,
            "port": args.port,
            "scan-interval-ms": 1000,
            "sources": sources,
        }

        with open(args.output, "w", encoding="utf-8") as f:
            yaml.safe_dump(
                config,
                f,
                sort_keys=False,
                allow_unicode=True,
            )

        print(
            f"Generated {args.output} "
            f"with {len(sources)} sources."
        )

    except Exception as e:

        print(
            f"Error communicating with Beckhoff PLC: {e}",
            file=sys.stderr,
        )

        return 1

    finally:

        try:
            plc.close()
        except Exception:
            pass

    return 0


if __name__ == "__main__":
    sys.exit(main())
