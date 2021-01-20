# How it Works

Just to be sure if we talk about `D6` that means a six sided dice with numbered
sides from 1 to 6. A `D20` is a 20 sided dice with numbers from 1 to 20. And so
on...

A [sample space](https://en.wikipedia.org/wiki/Sample_space) is a set of all
possible outcomes.

A dice sequence is a sequence of roles. A sequence is written down as a list of the rolled numbers in braces.
E.g. (1 3 2) for the sequence of 3 roles, the first dice 1 the second 3 and the third 2.

A terminated dice sequence is a sequence of roles that lead to one outcome in the sample
space. Multiple terminated dice sequences may result in the same outcome.

# Basics

Lets look at a simple sample. 

> Role a D6 if you role a 6 you may role once more and add the second role to
> your first.

You can draw a diagram to visualize how the probability's are distributed:

![probability graph](images/simple-propability.png)

The Probability is written in the braces, before that is the rolled value. In
the next line is the result.

As you can see the the values from 1 - 5 have a probability of 1/6. The values
from 7 - 12 have a probability of 1/36. To get the probability of the 3 you just
need to divide one by the number of faces of the dice.

To get the probability of the 8, you first need to role a 6, as we've already
know that has a probability of 1/6. We then need to role a 2. The probability
for that is 1 / 6 (for the first 6) / 6 (for the followed 2) = 1/36.

From now on we will write down how we role using pseudo code. The example above
would be:
```
var role: int = D6
if role == 6
    role = role + D6

return role
```

First we role a 6 sided dice and store its value in the variable `role`. We then
check if the in `role` stored value is a 6. Then and only then we role a second
dice sum it together with the stored value and overwrite the value in `role`. At
the end we return the value in `role`.

# Simple solution

If we want to know all possible outcomes and there distribution, the first
approach one might try is to just test every possible combination of roles and
note the outcomes. This is actually how the first version of this project had
done it.

But if your roles become more complex and the number of dices increases we will
soon have to many calculations to finish them in an reasonable amount of time.

In addition there may be infinite possibility's we need to know when to stop and
in which order to test.

Let us change one detail from the first example. We may now role again every
time we role a 6 not only the first.

```
# The current role
var role: int 
# The sume of all roles
var sum: int
do {
    role = D6
    sume = sum + role
} while role == 6

return sum
```


Now we have an infinite amount of dices that may be rolled. If we try to search
the sample space deterministic always starting with the highest number of
an dice, we would never finish the first role.

To mitigate that we could say to choose the rolled side randomly and make sure to
never take the same choice again.

Such a list can look like following:

* 3 => 1/6
* 6,4 => 1/36
* 6,3 => 1/36
* 5 => 1/6
* 6,6,1 => 1/216
* ...

This may have another problem, while it is unlikely we may never role a 1 in the
first role. meaning we miss 1/6 of out sample space. 

We also need to search extensively until we have searched all or at least most
of the sample space. To illustrate this we look at the following sample. It will
calculate how long a fight will take. We try to attack and if we hit we reduce
the life by D6.

```
var attack:int = 14
var life: int = 30
var rounds: int = 0

do {
    round = round + 1
    if D20 <= attack
        life = life - D6
} while life > 0

return rounds
```

We try to approximate the number of dice sequences.

We start with all sequences that exist in the first round.

First the number of sequences if we miss. (15) (16) (17) (18) (19) and (20) in
total 4 sequences. In addition we have the sequence where we hit. (1 1) (1 2)
... (1 6) (2 1) ... (2 6) ... (14 6) I hope you don't mind that I haven't
written all combinations down, there are 84. Together with the missed we have 88
sequences for the first round.

In the second round we have the same number of sequences, that means for every
sequence of the first round we have an additional 88 sequences resulting in a
total of 7744 sequences. Some example of those 7744 sequences are (16 15) (18 3
4) (2 4 18) (2 1 10 3). The minimum number of rounds is 5 if we hit every round
and role a 6 for dame we will have accumulated 30 damage. Or reduced the life to
zero. The amount of sequences we need to compute for 5 rounds is 88^5=
5,277,319,168.


# Solution in this project

We try to reduce the amount of computation by calculating intermediate results
and reduce those.

If we role 2D6 we have 36 possible dice sequences. But if we are only
interested in the sum of those 2 roles we reduce this to 11 outcomes (2 - 12).

```
var r1: int = D6
var r2: int = D6
var sum: int = r1 + r2
return sum
```
## The Variable tables

Instead of trying every combination of roles, we create a table for every
variable. This table contains the possible values the variable might have and
there probability's.

If we look at line 1, we crate one table it has two columns one column named r1
that contains the values r1 may have and a second column P that contains the
probability for the row.

r1|p
-|-
1|1/6
2|1/6
3|1/6
4|1/6
5|1/6
6|1/6

In line 2 we create a second table for r2.

r2|p
-|-
1|1/6
2|1/6
3|1/6
4|1/6
5|1/6
6|1/6

Those two tables are unrelated for now so we have two tables with both 6 values.

In line 3 we will now combine those two tables since the new variable sum is
depended on both tables. Instead of creating a third table we combine the
previous two tables resulting in one table that has the columns r1, r2, sum and
p

r1|r2|sum|p
-|-|-|-
1|1|2|1/36
2|1|3|1/36
3|1|4|1/36
4|1|5|1/36
5|1|6|1/36
6|1|7|1/36
1|2|3|1/36
2|2|4|1/36
3|2|5|1/36
4|2|6|1/36
5|2|7|1/36
6|2|8|1/36
1|3|4|1/36
2|3|5|1/36
3|3|6|1/36
4|3|7|1/36
5|3|8|1/36
6|3|9|1/36
1|4|5|1/36
2|4|6|1/36
3|4|7|1/36
4|4|8|1/36
5|4|9|1/36
6|4|10|1/36
1|5|6|1/36
2|5|7|1/36
3|5|8|1/36
4|5|9|1/36
5|5|10|1/36
6|5|11|1/36
1|6|7|1/36
2|6|8|1/36
3|6|9|1/36
4|6|10|1/36
5|6|11|1/36
6|6|12|1/36

In line 4 we now return the values for sum we can now return every outcome for
sum together with its probability.

## Optimizing Tables

But so fare we are not really better then version one. It doesn't matter if we try every possible combination or if we need to calculate every row.

To optimize our tables we need to know what variables we actually need. For that we start from the bottom, our return statement and for every line note what variables we actually need.

Line 4 needs the variable `sum`. Line 3 creates `sum` and uses for that `r1` and `r2`. So  line 3 needs `r1` and `r2`. Since it creates `sum` itself it no longer need the variable `sum`. Line 2 creates `r2` so it will no longer need it but since it does not provide `r1` it still requires `r1`. Line one does not require any variable.

line number | provides | require
-|-|-
1|`r1`| ∅
2|`r2`| `r1`
3|`sum`| `r1` `r2`
4| ∅ | `sum`

To optimize a table we can look at what the next line requires and remove the
columns in the table not required by the next line.

In our example, since line 4 does not require `r1` and `r2` we can delete those columns from our table.

sum|p
-|-
2|1/36
3|1/36
4|1/36
5|1/36
6|1/36
7|1/36
3|1/36
4|1/36
5|1/36
6|1/36
7|1/36
8|1/36
4|1/36
5|1/36
6|1/36
7|1/36
8|1/36
9|1/36
5|1/36
6|1/36
7|1/36
8|1/36
9|1/36
10|1/36
6|1/36
7|1/36
8|1/36
9|1/36
10|1/36
11|1/36
7|1/36
8|1/36
9|1/36
10|1/36
11|1/36
12|1/36

After deleting the columns we can group rows together that are identical (two rows are identical iv every entry in it is the same as in the other ignore the p column for that purpose) the p value is the sum of the grouped p values.

in our example this reduces the number of rows to 11.

sum | p
-|-
2| 1/36
3| 2/36
4| 3/36
5| 4/36
6| 5/36
7| 5/36
8| 5/36
9| 4/36
10| 3/36
11| 2/36
12| 1/36

Of course we need to calculate every of the 36 rows in order to calculate the sum of p. 
The advantage will be visible if we extend the sample with a third role

```
var r1: int = D6
var r2: int = D6
var r3: int = D6
var sum1: int = r1 + r2
var sum2: int = sum1 + r3
return sum2
```

The tables of `r1` `r2` `r3` start with 6 rows each. Combining `r1` and `r2` to
calculate `sum1` will create a table with 36 entry's. Before we combine `sum1` with `r3` we can optimize the table and reduce the number of rows to 11. So we only need to calculate 66 rows (6 * 11) instead of 216 ( 36 * 6).


Since we backtrack from the result to find the variables we need, we also eliminate dead variables.

```
var noLongerUsed: int D1000
var r1: int = D6
var r2: int = D6
var sum: int = r1 + r2
return sum
```

In this sample the first variable does not increase the complexity. Since Line 2 does not require `noLongerUsed` its table can be ignored.

## Loops



To be continued...