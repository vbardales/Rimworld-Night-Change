# Protocols read

Read 2026-09-29 by the Night Change session. Version = last commit touching the file (protocol history), then content hash (SHA256, first 12); all unmodified in git. Re-read a document only when its hash changes or its trigger below occurs.

## Useful (read fully, applied)

| Document | Version | Used for |
| --- | --- | --- |
| AGENTS.md | 90d51374 2026-09-25, 7A236F03CA15 | Gate order, test-evidence rule, CI publishing rules |
| AUDIT.md | 90d51374 2026-09-25, 0FB60FDF8C87 | Transitions, stage/workflow_stage, session title, `done -> tested` gates, evidence |
| MOD_SETTINGS.md | 90d51374 2026-09-25, 404916BC99A7 | settings_audit contract, hidden MainButton |
| TRANSLATIONS.md | 90d51374 2026-09-25, 298F74D226DA | l10n gate, plural rule |
| PUBLISHING.md | 90d51374 2026-09-25, 513F110AE0B6 | Leaving the origin project, PublishedFileId, CHANGELOG 0.1.0, CI publishing |

## Partially useful

| Document | Version | Trigger to re-read |
| --- | --- | --- |
| Rimworld-Release-Admin/docs/OPERATIONS.md | 3c03f51 2026-09-26, 23FCF6423000 | Before any workflow, tag, release or Steam secret (prepublished / publish) |
| STYLE_RIMWORLD.md | 90d51374 2026-09-25, 2C6DB32396AE | Any Preview/ModIcon change (owner generates the icon; sessions only check) |
| WORKSHOP_COMMENTS.md | 08878789 2026-09-29, 3FB37586F04B | Drafting thank-yous: Harmony is `posted` (add to Covers), Shift Change needs an entry |
| Rimworld-Ticket-Dispatcher/docs/WELCOME.md | 77ca9d7 2026-09-27, 08B440A03F74 | First Pickle ticket (REGISTER, no watcher) |

## Not useful yet (summarised, not applied)

| Document | Version | Trigger |
| --- | --- | --- |
| scripts/SEARCHING.md | 90d51374 2026-09-25, 013075B06B89 | A defName collision or Harmony conflict to search; never grep the corpus |
| PickleTools/README.md | c771bef 2026-09-25, 628350C7BCC3 | Deciding the Pickle suite |
| PickleTools/Headless/README.md | ed4e73a 2026-09-26, 2310BB974F68 | First Pickle pass / pass maps |
| PickleTools/docs/steps.md | da7c3b0 2026-09-28, DF2B37A6AFF2 | Choosing steps |
| PickleTools/Authoring/README.md | 8d3ca6d 2026-09-26, E620DF7E25D2 | Writing the first suite |
| Rimworld-Ticket-Dispatcher/docs/SUBMIT.md | d07b2b8 2026-09-26, EACA3969C7EB | First `Submit-PickleRun.ps1` |

## Mod documents

Present: STATUS.md, README.md, CHANGELOG.md, ATTRIBUTION.md, LICENSE, TESTING.md, BACKLOG.md, TEST_SCENARIOS.md, Mod/About/About.xml, docs/runs/. Absent, not needed yet: PUBLICATION.md (before prepublished), NOTES.md, BUGS.md, Tests/Pickle/.
