# Publishing Night Change

Publication sheet for Workshop item `3806765493`. The item exists and is **private** (prepublished as 0.1.0, the ID file is dated 2026-09-23). The mod is public on GitHub (`vbardales/Rimworld-Night-Change`, MIT). Steam publishes through CI (`PUBLISHING-CI.md`); nothing here is sent by a session. Target version: `1.0.0`. Nothing below is final until the French review and the game tests are done (`STATUS.md`).

## 1. Steam description

The Markdown block below is the single source. The CI converts it to Steam BBCode, and the `<description>` of `Mod/About/About.xml` is generated from it (`sync-about-description.mjs`). No RimWorld version number appears in it: the number lives in `supportedVersions` only.

## Steam description

```markdown
REQUIRES ODYSSEY (for the outfit stand) AND HARMONY.

Colonists change into night clothes on their way to bed, and back out of them when they get up.

Put a vanilla outfit stand in a bedroom and hang a set of night clothes on it. When the colonist who sleeps there goes to bed of their own accord, they stop at the stand, change, and go to sleep. Their day clothes wait in the stand overnight and come back exactly as they were, force-worn markers included.

A stand in a private bedroom needs no setup at all: it serves whoever owns the bed in the room. Set owners on it for a barracks, where several colonists could otherwise lay claim to the same stand.

**Why this is worth a mod**

Nothing dresses a colonist for bed. Pyjama mods exist and say so themselves: they ship the clothes and leave the wearing to you, which in practice means never. [Shift Change](https://steamcommunity.com/sharedfiles/filedetails/?id=3783456242) dresses colonists for the room they work or relax in and states plainly that it leaves the bed alone, because vanilla gives patients recreation they can take lying down and nobody should be pulled out of bed for a wardrobe. This mod is the other half, and the two sit side by side without touching: a bedroom is not a work room, so neither claims the other's stand.

The point is not the clothes. It is that going to bed becomes a thing a colonist does rather than a state they enter. Night clothes usually trade work speed and armour for comfort, which the game already prices; the mod simply makes the trade happen at the right hour without you clicking.

**What it will not do**

Deliberately narrow, so it never fights you.

- Automatic sleep only. A direct order to lie down is never delayed by a wardrobe trip.
- Medical rest is left alone. A colonist the game sends to bed to be treated goes straight there.
- No changing during a raid or a fire. Changing back is allowed then, because that is a colonist walking towards their own gear rather than away from it.
- Nobody is pulled out of bed. Watching television in bed, meditating in bed and resting after surgery all leave the night clothes on; the change back happens when they get up for good.
- Getting dressed happens before the day's work, but after fleeing a fire, after seeking a safe temperature and after emergency work.

**The cold**

Unlike a lab coat, night clothes replace what the colonist is wearing rather than layering over it, so a full change can strip a colonist of their insulation. Sleeping in a freezing bedroom in nothing but a nightshirt is a real way to die, and vanilla does not protect against it because nothing normally sends anyone there.

So the mod checks. It compares the insulation the night clothes give against the insulation the day clothes gave, applies the difference to the colonist's own comfortable minimum, and refuses the change when the bedroom is colder than that. The safety margin is adjustable, and the whole check can be turned off.

**Settings**

- Unassigned stands serve the owner of the bed in the room. On by default.
- Refuse the change when the bedroom is too cold, with an adjustable margin.
- Maximum distance between bed and stand.

They are under Options, Mod options, Night Change. English and French.

Adds no clothing, no buildings and no art of its own. It uses the vanilla outfit stand and whatever you hang on it. Vanilla has nothing that reads as night clothes, so any apparel mod that adds pyjamas, robes or nightgowns is worth having alongside.

**If I go quiet**

If I do not answer within a reasonable time after being contacted, anyone may freely update this or any other of my mods, including publishing a continuation of it. All credit must be preserved.

**AI-generated**

Written with the help of an AI assistant (Claude Code, Anthropic), under human direction and review. The preview image and the mod icon were generated with an AI image tool.

**Thanks**

Thanks to Andreas Pardeike for [Harmony](https://steamcommunity.com/sharedfiles/filedetails/?id=2009463077), and to MrBeverage, whose [Shift Change](https://steamcommunity.com/sharedfiles/filedetails/?id=3783456242) is MIT licensed and ships its own design notes. Nothing of his mod is reused here, but those notes documented a series of outfit stand pitfalls this mod would otherwise have found in play, and ATTRIBUTION.md lists them one by one.

[Pickle](https://steamcommunity.com/sharedfiles/filedetails/?id=3791648678), [RimLogging](https://steamcommunity.com/sharedfiles/filedetails/?id=3733484696) and [PickleTools](https://steamcommunity.com/sharedfiles/filedetails/?id=3806142401) were used for testing only; none is a dependency of the distributed mod.

Credits and licence: see ATTRIBUTION.md and LICENSE (MIT) in the repository.

[Source code on GitHub](https://github.com/vbardales/Rimworld-Night-Change)
```

## 2. Images

- `Mod/About/Preview.png`: header image, 896 x 504, under 1 MB. Sent only with `update_preview`.
- `Mod/About/ModIcon.png`: 128 x 128, the mod-list icon, not a gallery image.
- Gallery (uploaded by hand, the CI never sends it), `Art/Gallery/`, page order. **Pending: `shootGallery` has not run.** Today the folder holds only `0-preview.png`, a byte-for-byte copy of the Preview. Planned captures, one scenario each, each under 2 MB and 8 MB in all, to be confirmed against what the scenarios really show:

| Index | Shows | Scenario |
| --- | --- | --- |
| `0-preview.png` | the Preview | copy of `About/Preview.png` |
| `1-...` | a bedroom with a stand holding night clothes, colonist walking to it before bed | evening change |
| `2-...` | the same colonist asleep, day clothes parked on the stand | overnight |
| `3-...` | the "Set sleeper" and "Change back now" buttons on the stand | stand gizmos |
| `4-...` | the settings window | Mod options, Night Change |

## 3. Dependencies to declare on Steam

Required (`modDependencies` in `About.xml`): Harmony (`brrainz.harmony`, Workshop `2009463077`) and Odyssey (`Ludeon.RimWorld.Odyssey`, the outfit stand). Biotech (`Ludeon.RimWorld.Biotech`) is conditional: the kid outfit stand is patched when it is present, and the mod works without it. Shift Change (`MrBeverage.ShiftChange`) and Outfit Stands Plus (`khamenman.OutfitStandsPLus`) are `loadAfter` only, for a deterministic comp order on the shared stand def; neither is a dependency, and no `LoadFolders.xml` exists. Items to add as "Required items" on the Workshop page: Harmony only (Odyssey is a DLC).

## 4. Steam comments

Registry: `WORKSHOP_COMMENTS.md` (global). Post nothing before the item is visible to its readers (it is private today). Posting is Virginie's; no session posts.

| Recipient | Workshop id | Registry state | Local state |
| --- | --- | --- | --- |
| Harmony (Andreas Pardeike) | `2009463077` | `posted`, confirmed 2026-09-22 | covered: add Night Change to `Covers`, do not repost |
| Shift Change (MrBeverage) | `3783456242` | absent: add a `drafted` row | draft below |
| Outfit Stands Plus (khamenman) | `3545172389` | absent | `not_applicable`: the item was taken down, no public page to comment on; the fork `3724311713` has no established packageId and is not named in the description |
| Pickle, RimLogging, PickleTools | `3791648678`, `3733484696`, `3806142401` | to read in the register before posting | development tools named in the description: check their rows, add this mod to `Covers` if `posted` |

The last comments of the Shift Change page were not read. Read them before posting (language, mood, whether the author answers). Only what is true is said: the two mods were never run together in a game, so the draft claims no compatibility.

**Shift Change, 3783456242** (MrBeverage). English:

```text
Hey! Your DESIGN.md on the outfit stand pitfalls saved me a pile of in-game surprises, especially the forced flag being wiped on every removal. I wrote a small mod for night clothes at the stand beside the bed, [url=https://steamcommunity.com/sharedfiles/filedetails/?id=3806765493]Night Change[/url], and your notes are credited in it. Thank you for writing them down :)
```

(The link works only once the item is visible. Until then post the text without it, or wait.)

## 5. Other Steam fields

- Adult-content questionnaire: the mod adds no clothing, no building and no art, and shows no nudity, sexual or violent content of its own. A colonist is shown changing by walking to a stand: no undressing animation is added. Answer "no" to every adult-content box; confirm on the page.
- Tags: `Mod` plus one tag per `<supportedVersions>` entry, sent only with `update_tags` (off by default).
- Incompatible items: none declared. Loads alongside Shift Change and Outfit Stands Plus by design; the combination is not tested in game.
- Visibility: private today. The CI never sends it.
- Preview and description: the CI sends them only with `update_preview` and `update_description`.

## 6. Update notes

The version must be above every existing `v*` tag (none exists yet; `0.1.0` was only the creation of the Workshop item).

## Steam change notes

### 1.0.0

```text
[b]1.0.0[/b]

[list]
[*]First release: colonists change into night clothes at the outfit stand on their way to bed, and back when they get up.
[*]Cold guard with an adjustable margin, settings under Mod options, English and French.
[/list]
```
