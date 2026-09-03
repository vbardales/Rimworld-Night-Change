# Changelog

Format inspired by [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).
This file serves the repository and the Steam release notes; RimWorld does not display it in game.

## [1.0.0] — unreleased

First release. RimWorld 1.6. Requires **Odyssey** (for the outfit stand) and **Harmony**.

### The evening change

- A colonist going to bed of their own accord stops at the stand in their bedroom, puts on whatever
  is hanging there, and goes to sleep. Their day clothes wait in the stand and come back exactly as
  they were, force-worn markers included.
- A stand with no owner set serves whoever owns a bed in the same room, so a private bedroom needs
  no setup. The "Set sleeper" gizmo is for barracks, where several colonists could otherwise lay
  claim to the same stand.
- A full change is the behaviour, not an option: night clothes replace what is worn, they do not
  layer over it.

### The morning return

- Placed on `Humanlike_PreMain`: after the allowed area, safe temperature, emergency work and the
  apparel optimizer; before ordinary work and recreation. A fire comes before the trousers; the
  trousers come before the workbench.
- While the timetable says "sleep", the colonist stays in night clothes: a midnight snack does not
  trigger three trips to the stand.
- Nobody is pulled out of bed. Television in bed, meditating lying down and medical rest all leave
  the night clothes on.
- A "Change back now" button on the stand, to force the return.

### The cold guard

- The mod compares the insulation of the night clothes against that of the day clothes being
  parked, applies the difference to the colonist's comfortable minimum, and refuses the change if
  the bedroom is colder. Adjustable margin, and the guard can be turned off.

### What it refuses to do

- A direct order to lie down is never delayed by a detour.
- Medical rest is left alone.
- No changing during a raid or a fire; the return trip stays allowed, a colonist leaving their
  night clothes being a colonist walking toward their own gear.

### Living alongside other mods

- Mod-prefixed scribe keys and no hotkey binding, so the stand def can be shared with Shift Change
  and Outfit Stands Plus.
- Biotech's kid outfit stand is treated like the ordinary one.

### Settings

- Unassigned stands serve the bed's owner (on).
- Refuse the change when the bedroom is too cold (on), with a margin in degrees.
- Maximum distance between bed and stand.
