#!/bin/bash

mongosh <<EOF
var config = {
	"_id": "config-replica-set",
	"configsvr": true,
	"version": 1,
	"members": [
		{
			"_id": 0,
			"host": "mongo-config-01:27017",
			"priority": 1
		},
		{
			"_id": 1,
			"host": "mongo-config-02:27017",
			"priority": 0.5
		},
		{
			"_id": 2,
			"host": "mongo-config-03:27017",
			"priority": 0.5
		}
	]
};
rs.initiate(config, { force: true });
EOF