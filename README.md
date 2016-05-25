[![NuGet](https://img.shields.io/nuget/v/DiceTool.svg?style=flat-square)](https://www.nuget.org/packages/DiceTool/)
[![Build status](https://ci.appveyor.com/api/projects/status/0x4c9eocnkkn7ki7?svg=true)](https://ci.appveyor.com/project/LokiMidgard/dice-tool)
[![GitHub license](https://img.shields.io/github/license/LokiMidgard/Dice-Tool.svg?style=flat-squar)](https://tldrlegal.com/license/mit-license#summary)

# Dice-Tool

This library will help you calculating provability's of dice rolls.

# Sample

To use this toolkit you have to implement your dice logic in a class derived from the abstract
```DiceCalculator<T>```. This class provide Properties for the different dices called ```D2```
to ```D100```. The following simple implementation simulates a single role with a D6, it is a
success if the rolled number is greater or equal to 5.

```c#
    class SimpleDiceRole : Dice.DiceCalculator<bool>
    {
        protected override int RoleCalculation()
        {
            return D6 >= 5;
        }
    }
```

To use the just implemented class we call the ```DoIt()```-Methode on ```SimpleDiceRole``` which returns a
list of all possible results together with the probability.

```c#
	var simpleDiceGenerator = new SimpleDiceRole();
	var results = await simpleDiceGenerator.DoIt();
	foreach (var r in results)
		Console.WriteLine($"{r.Result}: {(r.Factor * 100):f2}%");
```
The result should be following:
```
True: 33,33%
False: 66,67%
```

A more complicated version is the next sample. First instead of a simple success or failure we return the
rolled number. Second if we role a three we will role again and add this role with all previous rolls.

```c#
    class RepeatingDiceRole : Dice.DiceCalculator<int>
    {
        protected override int RoleCalculation()
        {
            int result = 0;
            int role;
            do
            {
                role = D6;
                result += role;
            } while (role == 3);
            return result;
        }
    }
```
Unlike the first sample this has an infinite number of possibility's. If we would role only threes we would never
stop. In order to get our results we need to abort the calculation after some time.
```
	var repeatingDiceGenerator = new RepeatingDiceRole();
	var tokenSource = new System.Threading.CancellationTokenSource();
	tokenSource.CancelAfter(2000);
	var results = await repeatingDiceGenerator.DoIt(tokenSource.Token);
	foreach (var r in results)
		Console.WriteLine($"{r.Result}: {(r.Factor * 100):f2}%");
```
Every time a dice object stored in the Properties D2 to D100 is cast to an int, it is consider as a role.
So it is **important** that you **don't** use the ```var``` keyword.
The library determines the next role randomly but never creates a two identical combinations of rolls. 

This should prevent missing important possibilitys. For example if we would calculate all dice beginning
with 1 and going up to the highest number, we would never get any result where we had rolled a 4 or up.
This would mean missing the 16,66% chance for rolling the 6.

If all possible combinations have been found, the ```Task``` returned by ```DoIt()``` is finished.