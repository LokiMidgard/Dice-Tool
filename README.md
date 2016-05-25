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
		Console.WriteLine($"{r.Result}: {(r.Propability * 100):f2}%");
```
The result should be following:
```
True: 33,33%
False: 66,67%
```

## Exploding Dices
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
```c#
	var repeatingDiceGenerator = new RepeatingDiceRole();
	var tokenSource = new System.Threading.CancellationTokenSource();
	tokenSource.CancelAfter(500); // should be enough time to get the lowest 19 results.
	var results = await repeatingDiceGenerator.DoIt(tokenSource.Token);
    foreach (var r in results.OrderBy(x=>x.Result).Take(19))
		Console.WriteLine($"{r.Result}: {(r.Propability * 100):f2}%");
```
Every time a dice object stored in the Properties D2 to D100 is cast to an int, it is consider as a role.
So it is **important** that you **don't** use the ```var``` keyword.
The library determines the next role randomly but never creates a two identical combinations of rolls. 

This should prevent missing important possibility's. For example if we would calculate all dice beginning
with 1 and going up to the highest number, we would never get any result where we had rolled a 4 or up.
This would mean missing the 16,66% chance for rolling the 6.

If all possible combinations have been found, the ```Task``` returned by ```DoIt()``` is finished.

```
1: 16,67%
2: 16,67%
4: 19,44%
5: 19,44%
6: 16,67%
7: 3,24%
8: 3,24%
9: 2,78%
10: 0,54%
11: 0,54%
12: 0,46%
13: 0,09%
14: 0,09%
15: 0,08%
16: 0,02%
17: 0,02%
18: 0,01%
19: 0,00%
20: 0,00%
```

## More Parameters

You can also have multiple parameter to test against. Thes toolkit supports up to 5 parameters defined as
generic type arguments.
```c#
    class MultiDiceRole : Dice.DiceCalculator<int, int>
    {
        protected override int RoleCalculation(int numberOfDices)
        {
            return numberOfDices * W6;
        }
    }

```
This sample have an additional parameter of the type int. The concrete value will be provided as methode parameter.
Also note that multipling an int with a die means rolling that many dices. 

```c#
	var multiDiceGenerator = new MultiDiceRole();
	var results = await multiDiceGenerator.DoIt(new int[] { 1, 2, 3 });
	foreach (var f in results.GroupBy(x => x.Item1).OrderBy(x=>x.Key))
	{
		Console.WriteLine($"# of dice: {f.Key}");
		foreach (var r in f.OrderBy(x=>x.Result))
			Console.WriteLine($"\t{r.Result}: {(r.Propability * 100):f2}%");
	}
```
When we call ```DoIt()``` we need to provide all values that will be used.

```
# of dice: 1
        1: 16,67%
        2: 16,67%
        3: 16,67%
        4: 16,67%
        5: 16,67%
        6: 16,67%
# of dice: 2
        2: 2,78%
        3: 5,56%
        4: 8,33%
        5: 11,11%
        6: 13,89%
        7: 16,67%
        8: 13,89%
        9: 11,11%
        10: 8,33%
        11: 5,56%
        12: 2,78%
# of dice: 3
        3: 0,46%
        4: 1,39%
        5: 2,78%
        6: 4,63%
        7: 6,94%
        8: 9,72%
        9: 11,57%
        10: 12,50%
        11: 12,50%
        12: 11,57%
        13: 9,72%
        14: 6,94%
        15: 4,63%
        16: 2,78%
        17: 1,39%
        18: 0,46%
```