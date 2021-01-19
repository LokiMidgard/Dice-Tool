
This Sample simulates a Fight between two oponents.

Both Oponents will attack each other alternating. If a fighter hits, the defeneder tryes to parray, if he fails his life points will be reduced by D6 + 6.

The Fight goes on untill one fighter has no life points left.

The result will show who wins and in what round.

```
var attackA: int = 13
var attackB: int = 12
var defendA: int = 13
var defendB: int = 12
var lifeA: int = 27
var lifeB: int = 27

var counter: int = 0

do
{
  var hit: bool = false
  var d: int = W20
  hit switch d:
    >= attackA: false;
    default: true;
  var defend: bool = false
  d = W20
  defend switch d:
    >= defendB: false;
    default: true;
  var damage: int = 0
  damage switch hit & hit != defend:
    == true: W6 + 4;
    default: 0;
  lifeB = lifeB - damage
  var alive: bool
  alive switch lifeB:
    > 0: true;
    default: false;
  d = W20
  hit switch d:
    >= attackB: false;
    default: true;
  d = W20
  defend switch d:
    >= defendA: false;
    default: true;
  damage switch hit & alive & (hit != defend):
    == true: W6 + 4;
    default: 0;
  lifeA = lifeA - damage
  counter = counter + 1
}
while lifeA > 0 & lifeB > 0
var result: string = "NIX"
result switch lifeA - lifeB:
  < 0: "B Wins";
  > 0: "A Wins";
  default: "Draw";

# Uncomment the following line to also include when the fighter win
# result = result + " after "+ counter + " Rounds"

# Or the following to only see how long the fight will go on
# result = "The fight ends after " + counter + " Rounds"
return result

```