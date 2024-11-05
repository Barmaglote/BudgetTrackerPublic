#!/bin/bash

mongosh -u mongoadmin -p mongopassword <<EOF
var config = {
    "_id": "shard-02-data-replica-set",
    "version": 1,
    "members": [
        {
            "_id": 0,
            "host": "mongo-shard-02-arbiter:27017",
			"priority": 1
        },
        {
            "_id": 1,
            "host": "mongo-shard-02-data-01:27017",
			"priority": 0.5
        },
        {
            "_id": 2,
            "host": "mongo-shard-02-data-02:27017",
			"priority": 0.5
        }
    ]
};
rs.initiate(config, { force: true });
EOF