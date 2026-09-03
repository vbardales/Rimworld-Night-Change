# Night Change — la vitrine du mod

## Ce qu'il y a aujourd'hui

`About/Preview.png` : **absent**. Obligatoire côté Workshop (`PUBLISHING.md`).

`About/ModIcon.png` : **absent aussi**. Contrairement à Anima Song, il n'y a donc aucune palette
héritée à respecter, et rien n'est « déjà pris » : la bannière et le pictogramme se décident
ensemble, ci-dessous.

Une seule contrainte de famille : *For the Occasion* est le mod compagnon, et son pictogramme est
un **bol d'offrandes, or et ambre sur brun sombre**. Les deux icônes se retrouveront côte à côte
dans la liste des mods. Il faut donc qu'elles ne se confondent pas — d'où le bleu nuit ici, et un
sujet qui n'est ni un bol, ni une flamme, ni de l'encens.

## Les contraintes, reprises de tes autres mods

| | Format | Poids | Convention |
|---|---|---|---|
| `Preview.png` | **896 x 504** (16:9) | < 900 Ko (dur : < 1 Mo) | Architect Studio, Bill Autopilot, Animal Ark |
| `ModIcon.png` | 128 x 128 | ~20-30 Ko | rendu à ~32 px dans la liste des mods |

Contrainte dure côté Steam : la vignette s'affiche **autour de 268 px de large** dans les listes du
Workshop. Ce qui ne se lit pas à cette taille ne sert à rien.

## Ce que la vitrine doit dire

L'argument du mod n'est pas le pyjama : le mod **n'ajoute aucun vêtement**. L'argument, c'est le
**moment** — le colon s'arrête au portant en allant se coucher, et ses habits de jour y passent la
nuit. Ce qui se dessine, ce n'est donc pas un objet, c'est un **geste du soir**.

Les faits que l'image doit porter, dans cet ordre :

1. **Une chambre, la nuit, à l'intérieur.** Un lit, quatre murs, un toit, une lampe. C'est
   l'inverse exact d'Anima Song : ici tout est fermé, chaud et domestique. La règle du mod est que
   le portant doit être **dans la même pièce que le lit** (`StandFinder.ForBed` compare les
   `Room`), et c'est ça qu'on montre.
2. **Le portant à vêtements d'Odyssey, juste à côté du lit.** Pas une armoire : un petit mannequin
   sur pied, 1x1, qui présente **une seule tenue**. C'est structurel — le portant vanilla refuse
   tout vêtement qui ne se porte pas avec ce qu'il contient déjà.
3. **Les habits de jour sont sur le portant.** Manteau, chapeau. C'est le mécanisme entier en une
   image : ils y attendent la nuit et reviennent à l'identique au matin.
4. **Le colon est en tenue de nuit, encore debout.** Pas couché. Le lit est ouvert, prêt, vide.
   L'instant représenté est celui d'avant le sommeil.
5. **Une seule personne.** Le mod est intime et individuel — un portant, un lit, un dormeur.
   *For the Occasion* aura le rassemblement ; celui-ci a la chambre.

## Les écueils

* **L'armoire, la penderie, le dressing.** Le mod n'ajoute **aucun bâtiment**. Peindre une grande
  armoire vend un meuble qui n'existe pas, et efface le portant vanilla qui est tout le sujet.
* **Le catalogue de pyjamas.** Une rangée de chemises de nuit sur cintres, un étalage de tissus :
  ça dit « mod de vêtements », et c'est faux. Le mod ne fournit rien à porter — l'About le dit, il
  faut un mod d'habits à côté. Une vitrine qui promet des pyjamas déçoit à l'installation.
* **Le colon endormi.** Dormir existe déjà en vanilla. Si on peint quelqu'un au lit, on vend le jeu
  de base. Le pion est **debout, près du portant**, tenue de nuit sur le dos.
* **Le déshabillage.** Ni peau, ni intimité, ni scène de chambre. C'est une corvée du soir, pas un
  moment de vie privée. La solution est la même que sur Anima Song : **silhouette vue de dos**,
  sombre sur la lampe, ni visage ni détail — et ça se lit d'autant mieux à 268 px.
* **La robe de cérémonie.** C'est l'autre mod. Aucun apparat, aucune dorure, aucun rituel : ici on
  se met en chemise pour dormir.
* **L'atelier, l'établi, la blouse.** Territoire de Shift Change. Aucun outil, aucun poste de
  travail dans le cadre.
* **Le givre et la neige.** Le garde-froid est un **refus** — il empêche le changement quand la
  chambre est trop froide. Peindre du gel dirait « mod d'hiver » et promettrait une mécanique de
  température qui n'existe pas.
* **La bannière plus belle que l'écran.** Règle du dépôt : la vitrine ne montre jamais autre chose
  que ce que le joueur verra. Un lit, un portant, une lampe, un colon : tout est à l'écran.

---

# Le prompt Preview à coller dans l'IA d'image

> Demande une **image**, pas du SVG : c'est une illustration de vitrine, il n'y a ni palette à
> tenir ni aplats à respecter. Interdis-lui tout texte — le modèle en glisse spontanément dans les
> images de « mod banner ».

```
A wide 16:9 key art banner for a RimWorld mod called "Night Change", 896x504.

Concept: one settler getting changed for bed. It is night. In a small private bedroom, a colonist
has stopped at a slim clothes stand beside their bed, taken off their day clothes and left them
hanging there, and is now standing in simple loose night clothes about to go to sleep. The image
must read as a QUIET DOMESTIC EVENING ROUTINE — an ordinary chore before bed — never as a bedroom
scene, never as sleeping, never as a ceremony.

Composition: three-quarter view from a high camera angle, close to RimWorld's own overhead
perspective, wide shot of a small room seen from above and slightly to the side.
- Centre right: a narrow wooden clothes stand, like a slim tailor's mannequin on a single post,
  about the height of a person, standing on the floor. Hanging on it, clearly displayed: a heavy
  long coat and a hat — the settler's day clothes. Only ONE outfit on the stand, nothing else.
- Centre left: a single bed with the blanket folded back, empty, waiting. Plain wooden frame,
  simple cloth bedding.
- Between the bed and the stand: one settler standing, seen from BEHIND, a flat dark silhouette
  against the lamplight — only shoulders, back and legs, no face, no detail. They wear a simple
  loose long shirt for sleeping, plain and shapeless, fully covering. One hand still resting on
  the stand, body already half turned toward the bed.
- Setting: a small enclosed bedroom. Solid walls on two sides, a wooden floor, a closed door, a
  small window showing a dark blue night outside. A single oil lamp or small electric lamp on a
  side table is the only light source, casting a warm pool of light across the floor and throwing
  the settler into silhouette.
- Leave the upper left third dark and calm: the mod name will be laid over it later.

Style: painted digital illustration, matte, single warm light source. Restrained night palette —
deep blue and blue-grey shadow across the room, one warm amber pool of lamplight around the bed
and stand, the coat on the stand catching the warmest highlight. Readable at thumbnail size: one
bed, one stand with a coat on it, one dark standing figure, nothing else competing, no fine
texture detail that disappears when shrunk.

Absolutely no text, no letters, no numbers, no logo, no watermark, no UI frame, no border.
Do not render faces, eyes or any recognizable person: the figure is a dark silhouette seen from
behind, fully clothed in a long loose sleeping shirt.
Do not render any nudity, underwear, bare skin, or an undressing pose. Do not render a romantic
or intimate scene.
Do not render anyone lying down, sleeping, sitting on the bed, or under the blanket: the bed is
empty and the figure is standing.
Do not render a wardrobe, an armoire, a closet, a chest of drawers, a rail of clothes, a row of
hangers, or more than one outfit: there is exactly one narrow stand holding one coat and one hat.
Do not render a workshop, a workbench, tools, armour, weapons, a lab coat or a uniform.
Do not render a robe, a gown, jewellery, candles, incense, an altar or anything ceremonial.
Do not render snow, frost, ice or a fireplace.
Do not render a second person, a pet or an animal.
```

## Le prompt ModIcon

Le pictogramme prend **le portant et la lune**, pas le pyjama. Raison : le manteau accroché est ce
qui distingue ce mod d'un mod de vêtements — il dit « tes habits attendent là » — et le croissant
dit l'heure. Deux formes, ça tient à 32 px ; une silhouette en chemise de nuit, non. Et le bleu
nuit l'éloigne du bol or-sur-brun de *For the Occasion*, qui sera juste à côté dans la liste.

```
A simple flat icon, 128x128, for a RimWorld mod.

A narrow clothes stand seen from the front — a single vertical post on a small round base with a
short horizontal crossbar near the top — with one heavy coat hanging on it, and a small hat on
top of the post. Behind and to the upper right, a thin crescent moon.
Warm cream and pale amber shapes on a deep night-blue background.

Bold, chunky, high contrast, very few shapes, thick clean edges, no gradients, no fine detail:
it must stay legible when shrunk to 32 pixels.
Absolutely no text, no letters, no numbers, no border, no watermark.
No person, no face, no bed, no stars, no clouds.
```

## Après la génération

1. Recadrer la bannière en **896 x 504** exactement, puis réduire à **268 px de large** et
   regarder. C'est le vrai test. Ce qui doit survivre : le lit ouvert, le manteau sur son pied, la
   silhouette debout entre les deux. Si la silhouette se noie, il faut **assombrir la pièce**, pas
   éclaircir le colon.
2. Enregistrer en `About/Preview.png`, sous 900 Ko. Une image de nuit se compresse bien ; si elle
   dépasse, c'est du bruit dans les noirs, et un léger flou sur le mur du fond suffit.
3. L'icône en **128 x 128** dans `About/ModIcon.png`. Le piège documenté dans `PUBLISHING.md` :
   une icône de 1254 px pesait 1,9 Mo, soit 62 % du mod, pour une vignette de 32 px. Réduire
   d'abord, enregistrer ensuite.
4. Poser les deux à 32 px **côte à côte avec l'icône de For the Occasion** et vérifier qu'on les
   distingue d'un coup d'œil. C'est le seul test qui compte pour deux mods compagnons.

## Si le rendu déçoit

Ce qui casse le plus souvent : **le modèle transforme le portant en armoire**. Il connaît « bedroom
+ clothes » et sort un dressing. Repli : décrire l'objet sans le nommer — *« a single vertical
wooden post with a crossbar, like a coat rack, one coat hanging on it, nothing behind it »* — et
insister sur *« the wall behind the stand is bare »*.

Second défaut fréquent : **le colon se couche**. Dès qu'il y a un lit, le modèle y met quelqu'un.
Repli : *« the bed is completely empty, the blanket folded back, nobody touching it »*, et si ça
résiste, reculer le lit au second plan et cadrer sur le portant.

Troisième : **la scène devient intime**. Repli : remplacer la chemise de nuit par *« a plain
oversized tunic, shapeless, covering wrists and ankles »*, et redire la silhouette de dos.

Repli de composition, si la chambre ne se lit jamais en vignette : **plan serré sur le portant
seul**, manteau et chapeau dessus, un coin de lit ouvert qui entre par la gauche du cadre, la
lampe hors champ. On perd la pièce, on garde le mécanisme, et ça se lit beaucoup mieux à 268 px.
