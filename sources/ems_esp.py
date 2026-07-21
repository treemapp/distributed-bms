import requests


class Driver:

    def __init__(self, config):

        self.address = config["address"]
        self.endpoints = config["endpoints"]
        self.source_ids = {
            source["id"]
            for source in config["sources"]
        }

    def connect(self):
        return

    def read(self):

        sources = {}

        for endpoint_name, endpoint in self.endpoints.items():

            response = requests.get(
                self.address + endpoint["path"],
                timeout=5,
            )

            response.raise_for_status()

            self._flatten(
                endpoint_name,
                response.json(),
                sources,
            )

        return {
            key: value
            for key, value in sources.items()
            if key in self.source_ids
        }

    def _flatten(self, prefix, obj, sources):

        if isinstance(obj, dict):

            for key, value in obj.items():
                self._flatten(
                    f"{prefix}.{key}",
                    value,
                    sources,
                )

        else:

            sources[prefix] = obj

    def write(self, source, value):
        raise NotImplementedError("Writes not yet implemented")

