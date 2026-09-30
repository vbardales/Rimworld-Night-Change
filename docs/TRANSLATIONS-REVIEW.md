# Night Change - texts side by side

For the French review by Virginie (TRANSLATIONS.md section 3). Origin = the text as authored in `Mod/Defs/`; Keyed strings have no Def origin, their English file is the source. Files: `Mod/Languages/English/Keyed/NightChange.xml`, `Mod/Languages/French/Keyed/NightChange.xml`, `Mod/Languages/French/DefInjected/`. Revision 79686a9 (2026-09-30).

## Defs (DefInjected)

| Key | Origin (Defs) | English | Français |
| --- | --- | --- | --- |
| `NightChange_Settings.label` | Night Change | (Def value used) | Night Change |
| `NightChange_Settings.description` | Open Night Change settings. | (Def value used) | Ouvrir les réglages de Night Change. |
| `NightChange_ChangeAtStand.reportString` | changing at TargetA. | (Def value used) | se change à TargetA. |

## Keyed

| Key | English (source) | Français |
| --- | --- | --- |
| `NightChange_ModTitle` | Night Change | Night Change |
| `NightChange_SettingsScope` | Global settings, shared by all saves. Changes apply to the next decision to change clothes; a change already underway is not restarted. Settings are saved when this window closes. | Réglages globaux, communs à toutes les sauvegardes. Les changements s'appliquent à la prochaine décision de se changer ; un changement de tenue déjà en cours ne redémarre pas. Les réglages sont enregistrés à la fermeture de cette fenêtre. |
| `NightChange_SettingInheritOwner` | Unassigned stands serve the bed's owner | Un portant libre sert la personne qui possède le lit |
| `NightChange_SettingInheritOwnerDesc` | A stand with no owner set serves whoever owns a bed in the same room, so a private bedroom needs no setup. Turn this off to require an explicit owner on every stand. | Un portant sans propriétaire assigné sert la personne à qui appartient un lit de la même pièce : une chambre ne demande donc aucun réglage. Décochez pour exiger une assignation explicite sur chaque portant. |
| `NightChange_SettingColdGuard` | Refuse the change when the bedroom is too cold | Refuser le changement si la chambre est trop froide |
| `NightChange_SettingColdGuardDesc` | Night clothes replace day clothes rather than layering over them, so a change can cost a colonist their insulation. This compares the two and skips the change when the bedroom is colder than the colonist could stand afterwards. | La tenue de nuit remplace les habits de jour au lieu de se porter par-dessus : le changement peut donc coûter de l'isolation à la personne concernée. Cette option compare les deux et renonce quand la chambre est plus froide que ce que cette personne supporterait ensuite. |
| `NightChange_SettingColdMargin` | Safety margin: {0} °C | Marge de sécurité : {0} °C |
| `NightChange_SettingDistance` | Maximum distance from bed to stand: {0} cells | Distance maximale entre le lit et le portant : {0} cases |
| `NightChange_AssignGizmo` | Set sleeper | Choisir qui dort ici |
| `NightChange_AssignGizmoDesc` | Choose who changes at this stand before bed. Left unassigned, it serves whoever owns a bed in the same room. | Désigne qui se change à ce portant avant de se coucher. Laissé libre, il sert la personne à qui appartient un lit de la même pièce. |
| `NightChange_ChangeBackGizmo` | Change back now | Se rhabiller maintenant |
| `NightChange_ChangeBackGizmoDesc` | Interrupt {0} so they come and change back into their own clothes. | Interrompt {0} et l'envoie reprendre ses propres habits. |
| `NightChange_InspectInUse` | Holding {0}'s clothes | Contient les habits de {0} |

## Doubts for the reviewer

- `portant` for "outfit stand" (vanilla French name to confirm).
- `Choisir qui dort ici` for the button "Set sleeper" (reworded to avoid a masculine noun).
- `habits` versus `tenue` (both used).
- Gender agreement: no `{PAWN_gender}` switch was needed, every agreeing text was reworded (see `STATUS.md`, Translation audit).
