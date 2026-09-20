# Unarmed

**Status: backlog**

Genre: 3D arena action
Engine: Unity 6, URP
Target length: 20–30 minutes

---

## Hook

You cannot attack. You can only give their attacks back.

---

## The mechanic

There is no attack button. There is a parry.

A perfectly-timed parry **catches** an incoming strike and holds it.
Your next parry **throws** it — at a different enemy, at a wall, at the
one who threw it. Fights become juggling other people's violence.

- Catch a spear, throw it into the archer behind.
- Catch an arrow mid-flight, hold it while you catch a second, throw
  both.
- Mistime and you take the hit. There is no blocking, only catching.

The skill ceiling is in **routing** — which attack you choose to catch,
and who you aim it at — not in reflexes alone. A good player stands
still and lets the room kill itself.

---

## The story

You are the last priest of a god who forbade killing.

You keep that vow. You never strike anyone. You also walk out of every
room the only one standing, and the game never lets you forget that the
distinction is convenient.

Told through the arenas: shrines to a god of peace, each one a little
more damaged than the last. Prayers carved into walls that read
differently after what you have just done in front of them.

The final fight is against someone who fights the same way you do.
Neither of you can attack. Whoever runs out of patience first loses,
which is the entire argument of the game rendered as a boss.

---

## Why it works as a portfolio piece

- **Cheapest possible action game.** You need one player animation — the
  parry — instead of a whole attack moveset. The enemies supply every
  bit of spectacle.
- Best *feel* of the four if you land it. Nothing reads as skill like
  good combat.
- Very GIF-friendly: catch, turn, throw, enemy drops.

---

## Scope

**In:**

- One parry that catches and one that throws
- 3 enemy types: melee, thrower, archer
- 3 arenas
- 1 mirror-match boss
- No health pickups. Short fights, quick restarts

**Out:**

- Any attack of your own
- Weapons you keep
- Levelling, upgrades, skill trees
- Open areas — arenas only

---

## Risks

| Risk | Why it bites | What to do |
|---|---|---|
| Combat feel is brutal to tune | 80ms off and it feels broken, and only playtesting finds that | Build the parry window as a tunable number in the Inspector from day one. Expect to change it fifty times |
| Enemy AI is the hidden cost | "Cheap because no attack animations" hides the fact that three enemy types need three behaviours | Keep AI dumb and readable. Telegraph hard. Readable beats smart |
| One trick can get boring | Parry, parry, parry | Enemy variety carries the whole game. Each type must demand a different answer |

---

## First playable

1. One enemy that swings on a clear telegraph
2. Parry catches the attack
3. Second parry throws it back
4. The enemy can be killed only by its own weapon

If that single exchange feels good, the game works. If it does not, no
amount of enemies or arenas saves it.
