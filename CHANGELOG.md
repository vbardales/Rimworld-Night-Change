# Journal des modifications

Format inspiré de [Keep a Changelog](https://keepachangelog.com/fr/1.1.0/).
Ce fichier sert au dépôt et à rédiger les notes de version Steam ; RimWorld ne l'affiche pas en jeu.

## [1.0.0] — non publié

Première version. RimWorld 1.6. Exige **Odyssey** (pour le portant à vêtements) et **Harmony**.

### Le changement du soir

- Un colon qui part se coucher de lui-même s'arrête au portant de sa chambre, enfile ce qui y est
  accroché, et va dormir. Ses habits de jour attendent dans le portant et lui reviennent à
  l'identique, marqueurs de port forcé compris.
- Un portant sans propriétaire assigné sert celui à qui appartient un lit de la même pièce : une
  chambre privée ne demande aucun réglage. Le gizmo « Choisir le dormeur » sert aux dortoirs, où
  plusieurs colons pourraient sinon prétendre au même portant.
- Le changement complet est le comportement par défaut, et non une option : un pyjama remplace les
  habits, il ne se porte pas par-dessus.

### Le retour du matin

- Posé sur `Humanlike_PreMain` : après la zone autorisée, la température vitale, le travail
  d'urgence et l'optimiseur vestimentaire, avant le travail ordinaire et les loisirs. Un incendie
  passe avant le pantalon ; le pantalon passe avant l'établi.
- Tant que l'emploi du temps dit « sommeil », le colon reste en pyjama : un casse-croûte nocturne ne
  déclenche pas trois trajets au portant.
- Personne n'est tiré du lit. La télévision au lit, la méditation couchée et le repos médical
  laissent la tenue de nuit en place.
- Un bouton « Se rhabiller maintenant » sur le portant, pour forcer le retour.

### Le garde-froid

- Le mod compare l'isolation de la tenue de nuit à celle des habits déposés, applique la différence
  au minimum confortable du colon, et refuse le changement si la chambre est plus froide. Marge
  réglable, garde désactivable.

### Ce qu'il refuse de faire

- Un ordre direct de se coucher n'est jamais retardé par un détour.
- Le repos médical n'est pas touché.
- Aucun changement pendant un raid ou un incendie ; le retour, lui, reste permis, un colon qui
  quitte son pyjama marchant vers son propre équipement.

### Cohabitation

- Clés de scribe préfixées et liaison de raccourci retirée, pour partager le def du portant avec
  Shift Change et Outfit Stands Plus.
- Le portant pour enfants de Biotech est traité comme le portant ordinaire.

### Réglages

- Un portant libre sert le propriétaire du lit (activé).
- Refuser le changement si la chambre est trop froide (activé), avec marge en degrés.
- Distance maximale entre le lit et le portant.
