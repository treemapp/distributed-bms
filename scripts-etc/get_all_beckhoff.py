"""
Connect to a Beckhoff PLC and list all PLC symbols.

Usage:
    python get_symbols.py --ams-id 5.2.78.118.1.1 --ip-address 192.168.0.208
    python get_symbols.py --ams-id 5.2.78.118.1.1 --port 801 --ip-address 192.168.0.208

Port defaults to 851 (TwinCAT 3).
"""

import argparse
import sys

import pyads


def main():
    parser = argparse.ArgumentParser(
        description="List symbols exposed by a Beckhoff PLC."
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

        count = 0

        for symbol in symbols:

            # Ignore TwinCAT internal symbols
            if symbol.name.startswith("."):
                continue

            print(f"{symbol.name}\t{symbol.symbol_type}")
            #print(vars(symbol))
            count += 1

        print(f"\n{count} symbols found.")

    except Exception as e:
        print(f"Error communicating with Beckhoff PLC: {e}", file=sys.stderr)
        return 1

    finally:
        try:
            plc.close()
        except Exception:
            pass

    return 0


if __name__ == "__main__":
    sys.exit(main())



