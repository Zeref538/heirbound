# Exposure

**Status: backlog**

Genre: first-person 3D horror / exploration
Engine: Unity 6, URP
Target length: 30–45 minutes

---

## Hook

The world is pitch black. You see only the photographs you take.

---

## The mechanic

You hold a camera. Pressing the shutter fires a flash, and for a few
seconds the **still image stays projected in the space where you shot
it** — a frozen slice of the room hanging in the dark.

You navigate by your own photographs.

- A photo taken from the wrong angle is useless. You are looking at a
  picture of a wall.
- Photos fade. Move too slowly and the path behind you goes dark.
- Limited film. Every shot is a decision about where you are willing to
  be blind.

**And things move between photographs.** A shape at the end of the
corridor in one photo is closer in the next. The photo is the last thing
you know to be true, and it is already out of date.

---

## The story

You photograph buildings before demolition. This one is the last job.

The building objects to being documented. Not with noise or chasing —
with discrepancy. What is in the photo is not always what was in the
room. A door you photographed open is closed when you walk through it.
A figure appears in a shot of an empty hallway.

The horror is epistemological, which is a fancy way of saying: **you
cannot trust your only sense.** That is the whole feeling, and it comes
entirely from the mechanic.

Ending idea: the last roll of film shows you the building as it was
before, and you are in the photographs.

---

## Why it works as a portfolio piece

- **Darkness is free art.** You build one small building and light
  almost none of it. The scariest room is the one nobody modelled.
- It is the strongest **technical** showcase of the four — render
  textures, projection, lighting, post-processing.
- Instantly striking in a GIF. Nobody has to be told what they are
  looking at.

---

## Scope

**In:**

- One building, 8–12 small rooms
- Photo capture and projection
- Film limit and photo fade
- One entity that only moves when unobserved
- Sound design doing most of the tension

**Out:**

- Combat of any kind
- Multiple floors with complex navigation
- An inventory
- Jump scares — the mechanic is the scare

---

## Risks

| Risk | Why it bites | What to do |
|---|---|---|
| The photo-projection tech IS the game | If it does not work, there is nothing underneath | Prototype this first, alone, before any art. One room, one photo, one week |
| Render textures are memory-hungry | 16 GB RAM, several live photos at once | Cap active photos at 4–5, recycle the oldest |
| Navigating by photos may just be annoying | Frustration reads as bad design, not tension | Test on someone else early. If they rage, add a very dim ambient floor light |

---

## First playable

1. Pitch black room, camera in hand
2. Shutter captures what the camera sees and leaves it hanging in space
3. The photo fades after ~8 seconds
4. Walk around and navigate using only those frozen images

If moving through a dark room by your own photos feels tense rather
than tedious, the game works.
