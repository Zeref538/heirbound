# Cut Strings

**Status: backlog**

Genre: 3D physics puzzle / toy
Engine: Unity 6, URP
Target length: 30–45 minutes

---

## Hook

You are a marionette. You attach your own strings.

---

## The mechanic

There is no move key.

You click **a point on the ceiling** and then **a limb**, and a string
connects them and begins to shorten. That is your only verb. A string to
the right knee lifts the knee. Two strings and a shoulder and you can
lean forward until gravity takes over.

Everything else is physics. You are not driving a character; you are
wrestling a ragdoll into something that resembles walking.

- Strings snap under too much load.
- You have a limited number active at once, so moving means constantly
  cutting and re-attaching.
- Anchor points are where the theatre put them, not where you need them.

**The clumsiness is the design, not a bug.** Getting across a room is an
achievement in the first level and routine by the last, and the player
feels themselves getting better at a thing the game never explained.

---

## The story

The puppeteer is dead. The theatre is empty and the house lights are off.

Every level is a set from a play you used to perform — you know these
rooms, you have stood on these marks a hundred times, but always with
someone else holding the strings. Now the strings are yours and you do
not know how to use them.

The arc is literally the mechanic: **helpless, then capable, then free.**
The last level has no anchor points at all.

No dialogue. The story is in the sets, the playbills, and how much
better you have got at moving.

---

## Why it works as a portfolio piece

- **Unity's physics does the heavy lifting.** A small amount of code
  produces a lot of behaviour, which is the best ratio on this list.
- The most **shareable** of the four. Physics flailing is funny, and
  funny gets watched.
- "The awkwardness is intentional and the story is about it" is a pitch
  that makes a designer sound like a designer.

---

## Scope

**In:**

- One marionette ragdoll, properly jointed
- String attach / detach / shorten
- 6–8 small stage sets as levels
- Snapping strings, limited active count
- One theatre, dressed differently per level

**Out:**

- Any second character
- Combat
- Cutscenes
- Procedural anything

---

## Risks

| Risk | Why it bites | What to do |
|---|---|---|
| Ragdolls are unpredictable | Unity joints can explode for no visible reason | Tune joint limits early. Keep mass ratios between connected bodies sane — wild ratios are what cause explosions |
| "Charmingly hard" vs "unplayable" | A thin line, and you cannot see it yourself after a week | Put it in front of someone who has never played it, at week two, not week eight |
| Camera in a physics game | Player loses track of their own body | Lock the camera per-stage. Do not let the player fight the camera AND the puppet |

---

## First playable

1. Ragdoll marionette standing on a floor
2. Click ceiling point, click limb, string forms and shortens
3. Strings snap over a load threshold
4. Cross one room

If getting across that one room is satisfying rather than infuriating,
the game works.
