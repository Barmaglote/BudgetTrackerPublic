#!/bin/bash

mongosh <<EOF
use admin;
db.createUser({user:"mongoadmin", pwd:"mongopassword", roles:[{role: "root", db: "admin"}]});
exit;
EOF

mongosh -u "mongoadmin" -p "mongopassword" <<EOF
use admin;
db.createUser({user:"clusteradmin", pwd:"clusterpassword", roles: ["clusterAdmin", "readWriteAnyDatabase"]});
exit;
EOF

mongosh -u "mongoadmin" -p "mongopassword" <<EOF
use loginapi;
db.createUser({user:"loginapiadmin", pwd:"loginapipassword", roles:[{role: "readWrite", db: "loginapi"}]});
exit;
EOF

mongosh -u "mongoadmin" -p "mongopassword" <<EOF
use paymentapi;
db.createUser({user:"paymentapiadmin", pwd:"paymentapipassword", roles:[{role: "readWrite", db: "paymentapi"}]});
exit;
EOF