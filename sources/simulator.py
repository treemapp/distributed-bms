class Driver:

    def __init__(self, config):

        self.sources = {}

        for source in config["sources"]:

            self.sources[source["id"]] = source.get("initial")

    def connect(self):
        return

    def read(self):

        sp = self.sources["GT11_SP"]

        self.sources["GT11_PV"] += (
            sp - self.sources["GT11_PV"]
        ) * 0.2

        self.sources["TF1_PV"] = self.sources["TF1_OP"]

        self.sources["GT11_SENSOR_ERROR_FAULT"] = True

        return self.sources

    def write(self, source, value):

        if source not in self.sources:
            raise KeyError(source)

        self.sources[source] = value