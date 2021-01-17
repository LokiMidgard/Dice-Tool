[![NuGet](https://img.shields.io/nuget/v/DiceTool.svg?style=flat-square)](https://www.nuget.org/packages/DiceTool/)
[![Build status](https://ci.appveyor.com/api/projects/status/0x4c9eocnkkn7ki7?svg=true)](https://ci.appveyor.com/project/LokiMidgard/dice-tool)
[![GitHub license](https://img.shields.io/github/license/LokiMidgard/Dice-Tool.svg?style=flat-squar)](https://tldrlegal.com/license/mit-license#summary)

# Dice-Tool

This library will help you calculating provability's of dice rolls. Version 2 is a complete rewrite and not compatible to earlier versions.
It is no longer possible to just implement a class in C# and use normal local variables.
Instead you need to call and chain some library methods to represent your logic. This allows for higher performance since we have a greater
understanding of the shape of your code.

To make it easier to simulate your dices a DSL was also added.

# The Dice Language

## The binary operators

The langue uses [Infix notation](https://en.wikipedia.org/wiki/Infix_notation).

| operator | meaning |
|----------|---------|
|Add|`+`|
|Substract|`-`|
|Multiply| `*`|
| Divide| `/`|
| Modulo| `%`|
| (Bit)And| `&`|
| (Bit)Or| `|`|
| (Bit)XOr| `^`|
| Shift left| `<<`|
| Shift right| `>>`|
| Less or Equals| `<=`|
| Greater or Equals| `>=`|
| Less then | `<`|
| Greater then | `>`|
| Equality | `==`|
| Inequality | `!=`|


## Keywords

Following keywords are reserved and should not be used as variable names.

* `do`
* `while`
* `var`
* `if`
* `else`
* `return`
* `int`
* `bool`
* `string`
* `true`
* `false`
* `switch`
* `default`

## Constant Literals

You can write Integer (ℤ) and "Quoted strings" with backslash (`\`) as escape character and `ture` and `false`like in most languages.
In Addition you can also use a literal for dices. The literal is constructed as following: `D|W\d+` e.g. `D6` for a 6 sided dice or `W20` for a 20 sided dice.
It does not matter if you use `D` or `W`. `D` stands for dice and `W` for Werfel the German word for dice.


## Retrun

To actually get some output you need to return a value. A return statement looks like following:
```
return {expression}
```
E.g `return D6` which is a complete valid programm.

Only one return Statement is allowed and it must be the last statement.

## Variables

Before you can assign a value to a variable you need to define it.
```
var {name} : {type} [= {expression}]
```

`name` must be a valid identifier, meaning an alpha numeric word that does start with an character and is not a keyword and nor a dice.
`type` must be one of `int`, `bool` or `string`.
You may optionally assign a value at declaration time.

To assign a new value use `=`
```
{name} = {expression}
```
E.g to increment the variable `foo` you can write `foo = foo + 1`

## Loops

A Loop starts with the keyword `do` followed by a block that is marked with `{` and `}` and ends with a `while` followed by an condition, an expression that evaluates to a Boolean.

```
var sum :int = 0
do {
    sum = sum +1
} while sum < 10
```
This will increment sum until it reaches 10.

## Branches

> For performance reasons you should preferer the switch statement over the if statement. If possible.

A branch is created using the `if` keyword followed by an expression that evaluate to a `boolean`. After the expression comes the one statement.
A statement may be a block of statements surrounded by `{` and `}`. Optionally the  `else` keyword with another statement may end the branch.

```
var sum: int = W6
if W6 > 4 
  sum = sum + 7
```


## Switch

The switch statement evaluates an expression and compares it to several
conditions. For the first condition that was true it then evaluates this
conditions expression and assigns it to the variable of the switch statement.

```
{variable} switch {expresion}:
[{operator} {case expression}: {result expression};]+
default: {default expression};
```

`variable` is an already defined variable. `expression` is the input against
which the cases get evaluated. Each case has a `operator` which is a compare
operator like `==` or `<` followed by `case expression` against the comparison
is performed. You can have one or more cases.

After the cases you need a default that is used if no case did match.

You must not forget the `;` at the end of each case and the default. 

You should also preferer the switch statement over an if statement if possible.
It is more limited then an if/else. But many if statements can still be written
as a switch. These limitations allow for better optimsations that can cut
computation time in half.

As an example we simulate an attack, if we hit we role damage. The intuitive approach would probably be following:

```
var attackValue : int = 14
var damage: int = 0
if D20 <= attackValue
    damage = D6 + 4
```

But you can also write it with a switch instead
```
var attackValue : int = 14
var damage: int = 0
damage switch D20:
    <= attackValue: D6 + 4;
    default: 0;
```

The later one can be better optimized so it will not increase number of possible
results as much as the first. If you only have this one if statement it should
not be a problem. But if you have loops in your program this will get out of hand quickly.

I compared two sample codes that simulate a battle similar to TDE (The Dark
Eye). Two characters fight each other both have an attack a defense and life
points. If a character hits (rolling under its attack) and the other does not
defend (rolling over his defend) damage is substracted from the opponents life
points.

Both characters try to damage each other alternating until one characters life points reach 0.

The code returns the winner.

| style  | computation time      |
|--------|-----------------------|
| switch | 1 min 18 s            |
| if     | 30 min ~ 23% computed |

I stopped the if statement code after 45 min. It did not improved in the last 15
min. (not visibly)

<details>

 <summary>Code with switch statements</summary>

```
var attackeA :int = 13
var attackeB :int = 12
var verteidigungA :int = 13
var verteidigungB :int = 12

var lebenA:int = 10
var lebenB:int = 10

do{
var hit:bool = false
var d:int = D20
hit switch d:
>= attackeA : false;
default:true;

var defend:bool = false
d = D20
defend switch d:
>= verteidigungB: false;
default:true;

var damage:int=0
damage switch hit & (hit != defend):
==true: D6+4;
default: 0;

lebenB = lebenB-damage

var alive:bool
alive switch lebenB:
>0:true;
default:false;

d = D20
hit switch d:
>= attackeB : false;
default:true;

d = D20
defend switch d:
>= verteidigungA: false;
default:true;

damage switch hit & alive & (hit != defend):
==true: D6+4;
default: 0;

lebenA = lebenA-damage


} while (lebenA>0 & lebenB>0)

var result :string= "NIX"
result switch lebenA -lebenB:
<0:"B Gewinnt";
>0:"A Gewinnt";
default: "unentschieden";

return result

```

</details>

<details>

 <summary>Code with if statements</summary>

```
var attackeA :int = 13
var attackeB :int = 12
var verteidigungA :int = 13
var verteidigungB :int = 12

var lebenA:int = 10
var lebenB:int = 10

do{
var hit:bool = false
var d:int = D20

if d < attackeA 
hit = true

var defend:bool = false
d = D20
if d < verteidigungB 
defend = true

var damage:int=0

if hit & (hit != defend) 
damage = D6+4


lebenB = lebenB-damage

var alive:bool
if lebenB > 0 
alive = true
else
alive = false

d = D20
if d < attackeB 
hit = true
else
hit = false

d = D20
if d < verteidigungA 
defend = true
else
defend = false

if hit & alive & (hit != defend) 
damage = D6+4
else 
damage = 0

lebenA = lebenA-damage


} while (lebenA>0 & lebenB>0)

var result :string= "NIX"
result switch lebenA -lebenB:
<0:"B Gewinnt";
>0:"A Gewinnt";
default: "unentschieden";

return result

```

</details>

## Comments

You comment a line if you prepend it with an `#`. The `#` must not be in an statement.
E.g the switch statement normaly spans multiple lines. You must not write a comment in that.

# UI

There is a Windows Desktop application that allows "fast" testing. You can add
multiple dice programs and it will save the results if they where already
calculated.

You can write the code on the left side and the results will appear on the
right. There you also find a progress bar that shows the overall progress. It
shows how much of the probability space you have already visited.

But be aware that this is not an linear search. The first results found will
have a much higher probability to occur then the later ones. So the progress
will slow down.

The tool will also not search the completed probability space since like with
exploding dices there will be an infinite number of results. It will stop if it
has found more then 99.99% of the probability space.