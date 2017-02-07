# Coding Guidance

## Static *all* the things!

We were making most fields static within both HouseManager and XMLParser.
It *kinda* makes sense. After all, there's *definitely* only every going to be one of each!
However, if we want to do automated testing, it's *really* handy to be able to control the lifecycle of an object.

For example, with static:

```c#
public class Player
{
    public static int health = 100;
}

public class PlayerTester
{
    public void TestPlayerTakesDamage()
    {
        assert(Player.health == 100);
        
        Player.health -= 20;

        assert(Player.health == 80);
    }

    public void TestPlayerHeals()
    {
        assert(Player.health == 100); // OOPS!
    }
}
```

Without static:

```c#
public class Player
{
    public int health = 100;
}

public class PlayerTester
{
    public void TestPlayerTakesDamage()
    {
        Player player = new Player();
        assert(player.health == 100);
        
        player.health -= 20;

        assert(player.health == 80);
    }

    public void TestPlayerHeals()
    {
        Player player = new Player();
        assert(player.health == 100); // yay!
    }
}
```

Now, this is Unity, which means if it's a MonoBehavior object, we can't just use `new`, but
the syntax to instantiate is simple and thus not covered here.

**tl;dr:** Instead of saying "why shouldn't this be static", say "why *should* this be static?"


## Separation of Concerns

Let's look at something close to our parsing code.

```c#
public class XMLParser {
    public int health = 100;

    public List<GameObject> ParseHouse(XElement doc) {
        // ...
        List<GameObject> rooms = new List<GameObject>();
        // ...
        HouseManager.rooms = rooms;
        // ...
        return rooms;
    }

}
```

What if I wanted to re-parse the house doc, but *only* to get some item information, or something?
I can't! It'll reset all of the room state in the entire house! :o

Classes (and ideally, functions) should **do one thing, and do it well.**

An XML Parser should not be keeping track of anything related to the game itself.
In fact, it shouldn't keep any state at all! (This is one of those times when static is probaly okay)

A parsing method shouldn't be statically messing with some other object.

Overall, it's about making debugging easier. "Hmm, the house's rooms are getting messed up."
It's a lot easier to solve that problem if they're being assigned in *one* place instead of many,
especially if it's not clear *when* it happens.


## extension methods

```c#
class SomeoneElseMade
{
    public String firstName;
    public String lastName;
}
```

Aw darn. A `.FullName()` helper would have been really convenient here.
But we can't modify a class someone else made if it's not in our codebase!

... or can we?

```c#
public static SomeoneElseMadeExtensions
{
    public static String FullName(this SomeoneElseMade self)
    {
        return self.firstName + " " + self.lastName;
    }
}

var sem = new SomeoneElseMade();
sem.firstName = "John";
sem.lastName = "Madden";

sem.FullName(); // John Madden
```

## var, because typing

```c#
List<String> words = new List<String>();
```

Why did we have to repeat ourselves right there? The compiler *knows* it's a `List<String>`.

Turns out... we don't need to repeat ourselves.

```c#
var words = new List<String>();
```


## Lambdas

We spend a lot of time looking at each room and testing it in some way.
Why don't we abstract that to make it easier?

```c#

interface RoomTester
{
    public bool RoomMeetsCriteria(Room room);
}

// return true if the room has the right name
class RoomFinder : RoomTester
{
    public String name;

    public bool RoomMeetsCriteria(Room room)
    {
        return room.Name == name;
    }
}

public Room FindRoomBy(RoomTester tester) {
    for (int i = 0; i < rooms.Length; ++i) {
        if (tester.RoomMeetsCriteria(rooms[i])) {
            return rooms[i];
        }
    }
    return null;
}

Room room = FindRoomBy(new RoomFinder("Potato"));

if (room == null) {
    Debug.Log("No room named potato");
}
```

Well, that's a lot of typing when abstractions are supposed to *save* us effort.

In the end, we're doing one really simple thing: taking in a room and outputting a bool.

An instance of a class is a value. What if I told you... *functions* were values too?


```c#
// return true if the room has the right name
class RoomFinder : RoomTester
{
    public String name;

    public bool ByName(Room room)
    {
        return room.Name == name;
    }
}

public Room FindRoomBy(Func<Room, bool> predicate) {
    for (int i = 0; i < rooms.Length; ++i) {
        if (predicate.Invoke(rooms[i])) {
            return rooms[i];
        }
    }
    return null;
}

var isPotato = new RoomFinder("Potato");

Room room = FindRoomBy(isPotato.ByName);

if (room == null) {
    Debug.Log("No room named potato");
}
```

Nice! Who needs interfaces anyway?

Predicate is a math term-- it basically means "a test to see if something is true."
In fact, C# has this built-in! I could have said `Predicate<Room> predicate` instead.

Now, that class was still a whole lot of typing for a single string comparison.
But, we need some way to keep around that string. Otherwise we'd have to take another string argument
to `FindRoomBy` to pass it in, and then when we want to add searching by State we have to add one of those, etc...

But C# has `lambdas`! Lambdas are functions that aren't bound to a name when they're created, and can also
"capture" references to values outside of them. Or can just have static values inside of them.

The syntax is `(param) => { Debug.Log("In lambda"); return param * 2; }`

Or more commonly, when you just want one statement and only have one param,
`param => param * 2`.

So we can completely replace the above with

```c#

public Room FindRoomBy(Predicate<Room> predicate) {
    for (int i = 0; i < rooms.Length; ++i) {
        if (predicate.Invoke(rooms[i])) {
            return rooms[i];
        }
    }
    return null;
}

Room room = FindRoomBy(room => room.Name == "potato");

if (room == null) {
    Debug.Log("No room named potato");
}
```

There's even *more* ways we can save on typing here. See the LINQ section for that.


## Loop de Loop

There were many snippets of code like this:

```c#
for (int i = 0; i < rooms.Length, ++i) {
    if (rooms[i].Name == lookingFor) {
        for (int j = 0; j < rooms[i].States; ++j) {
            if (rooms[j].States[i].Name == stateLookingFor) {
                // ...
                return;
            }
        }
    }
}
```

That's a lot of repetition. And when there's room for repetition, there's room for making mistakes.
In fact, there's a mistake above. Do you see it? I swapped i and j! That'll be a crash eventually.

And, when something is 3 or 4 nested levels deep, and you need to read that section and compare it to the above section, etc. etc....
It's hard to reason about. Nesting is annoying!

One of the golden rules of programming is **don't repeat yourself**, or **D.R.Y.**

So let's clean this up, eh?

```c#
for (int i = 0; i < rooms.Length, ++i) {
    var room = rooms[i];
    if (room.Name == lookingFor) {
        for (int j = 0; j < rooms[i].States; ++j) {
            var state = room.States[j];
            if (state.Name == stateLookingFor) {
                // ...
                return;
            }
        }
    }
}
```

Okay, that's a little better. But C# has a way to make this even easier: `for-each` loops!

```c#
foreach (Room room in rooms) {
    if (room.Name == lookingFor) {
        foreach (State state in room.States) {
            if (state.Name == stateLookingFor) {
                // ...
                return;
            }
        }
    }
}
```

It's even shorter and even more *expressive*. It's clear what we're doing because it reads just like English!

Now, I know we're still dependent on item indexes in some places. That'll be talked about in another section.
So I've added a convenience extension method so we still can use the above sexy form!

```c#
foreach (KeyValuePair<int, Room> roomAndIdx in rooms.WithIndex()) {
    var i = roomAndIdx.Key;
    var room = roomAndIdx.Value;
    if (room.Name == lookingFor) {
        foreach (var stateAndIdx in room.States.WithIndex()) {
            if (stateAndIdx.Value.Name == stateLookingFor) {
                // ...
                return;
            }
        }
    }
}
```

Bleh, we went up in line usage again. We shouldn't be reliant on indexes anyway, this is just to make things cleaner
for those few times when you need it.

There's one last trick we're missing. Once you find the state in the room, you're done, right?
So why keep that loop around if you're done looking?

```c#
Room room = null;
foreach (var rm in rooms) {
    if (rm.Name == lookingFor) {
        room = rm;
        break;
    }
}

if (room == null) return;

State state = null;
foreach (var st in room.States) {
    if (st.Name == stateLookingFor) {
        state = st;
        break;
    }
}
// ...
```

No more nesting!

Okay, I lied. There's one more trick we can do to save on typing and be more expressive.
It combines aspects from the last two sessions. Let's move on to...


## LINQ

*Not just the hero of time.*

It stands for Language INtegrated Query. It's a set of extension methods that operate on 
IEnumerables (the thing that lets you do `for-each`. e.g. lists, arrays, dictionaries).

See the above example? We can replace *the entire thing* with:

```c#
var room = rooms.Find(rm => rm.Name == lookingFor);

if (room == null) return;

var state = room.States.Find(st => st.Name == stateLookingFor);

// ...
```

See the [DotNetPerls docs on LINQ](https://www.dotnetperls.com/linq) for more info.

## Attributes

Those `[BracketyThings]`. They allow us to attach metadata to almost anything in a class!
For instance, in the standard library, putting `[Deprecated]` on any function will result in a warning
if you try to use it.

Before, we were dispatching commands based on method names. If we had super duper full separation of concerns,
and all commands were contained in a `Commander` or something, that might make sense.

Well, until some jackass tries this:

```c#
RunCommand("ToString");
```

This is why I made the `CommandAttribute`. It lets you not only mark which methods are commands,
but specify alternate names for them too!


You can make your own attributes yourself easily. Just make a class whose name ends in `Attribute` (You leave that part off when you use it),
and subclass `Attribute`. Make a constructor and you can take arguments, like `[Command("potato")]`.
You restrict where you're allowed to use this by putting an attribute on the attribute class (how meta).
For example, `CommandAttribute` has `[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]` because
it only makes sense to put it on a function, and a command can have multiple names.


## Make strings not ints

We have cross-references within our XML like so:

```XML
<items>
    <item>
        <name>Potato</name>
    </item>
</items>
<rooms>
    <room>
        <items>
            <item>0</item>
        </items>
    </room>
</rooms>
```

This means that we're not repeating ourselves, which is good.

However, this is super brittle. What if someone adds another item *above* Potato, not realizing there was a reference to it?
The room will have the wrong item!

What if we just did this?

```XML
<room>
      <items>
          <item>Potato</item>
      </items>
</room>
```

It would be even *easier* to look up items in the codebase then. We could just make a `Dictionary<string, Item>`.


## if-return-bool

This is a completely meaningless thing and I apologize, but it's a pet peeve of mine.

```c#
if (thing < 2) {
    return true;
} else {
    return false;
}
```

Just do this:

```c#
return thing < 2;
```

Yes, I know. That's why I put it last. Sorry.


Also if you actually made it this far reading this, I love you.