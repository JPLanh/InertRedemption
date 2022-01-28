exports.newGame = function(newDeck){
	var mapper = {
		"blue1":{"index":0, "amount":5}, 
		"blue2":{"index":1, "amount":5},
		"brown":{"index":2, "amount":4},
		"compCookie":{"index":3, "amount":4},
		"drinksCarrier":{"index":4, "amount":2},
		"drinkSpill":{"index":5, "amount":4},
		"green2":{"index":6, "amount":5},
		"green3":{"index":7, "amount":3},
		"green4":{"index":8, "amount":2},
		"loyaltyCard":{"index":9, "amount":2},
		"orange2":{"index":10, "amount":5},
		"orange3":{"index":11, "amount":4},
		"orange4":{"index":12, "amount":3},
		"purp1":{"index":13, "amount":10},
		"purp2":{"index":14, "amount":5},
		"toGoCup":{"index":15, "amount":4},
		"yellow1":{"index":16, "amount":5},
		"yellow2":{"index":17, "amount":5},
		"yellow3":{"index":18, "amount":5}
	};

	var drawPile = [];
	Object.keys(mapper).forEach(
		function(key) {
			for (var cardAmt = 0; cardAmt < mapper[key].amount; cardAmt++)
				drawPile.push(mapper[key].index);
		}
	);
	shuffle(drawPile);
	shuffle(drawPile);

	newDeck(drawPile);
}

function shuffle(array){
	var currentIndex = array.length, temporaryValue, randomIndex;

  // While there remain elements to shuffle...
	while (0 !== currentIndex) {

    // Pick a remaining element...
 	randomIndex = Math.floor(Math.random() * currentIndex);
 	currentIndex -= 1;

    // And swap it with the current element.
 	temporaryValue = array[currentIndex];
 	array[currentIndex] = array[randomIndex];
  	array[randomIndex] = temporaryValue;
  }
  return array;
}
