const
	express = require('express'),
        app = express(),
	server = require("http").Server(app),
	io = require('socket.io')(server)
	socket = require('./API/Socket/socket'),
        port = process.env.PORT || 26843,
        mongoose = require('mongoose');

socket(io);

server.listen(port, function(){
	console.log("Starting up service " + port);
});

mongoose.Promise = global.Promise;
mongoose.connect('mongodb://localhost/bitDefenders');