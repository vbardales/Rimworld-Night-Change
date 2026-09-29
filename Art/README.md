# Art

- `ModIcon-source.png`, `Preview-source.png`: the generated originals, unchanged.
- `Preview-edit.md`: the 2026-09-13 accent correction of the Preview.
- `Preview-without-icon.png`: the Preview as delivered before 2026-09-29 (896 x 504, byte-identical to the old `Mod/About/Preview.png`), the input of the icon composite.
- `ModIcon-cutout.png`: the ModIcon with only its background made transparent (`_tools/add-icon-to-preview.cjs`: flood fill from the border, tolerance r<22 g<30 b<44, so the mascot's own outline and the moon and stars are kept).
- `Preview-with-icon.png`: the Preview with the cutout icon in the bottom-right corner, 210 px, tilted -15deg, bleeding 90 px off the right edge and 50 px off the bottom (as if coming out of the corner) (PUBLISHING.md rule of 2026-09-29: left corner +15deg, right corner -15deg). Installing it as `Mod/About/Preview.png`, and copying it as `00-` of the gallery, is done after the queued Pickle runs, so that the staged tree does not change under them.

Rebuild: `set NODE_PATH=<a node_modules holding sharp>` then `node _tools/add-icon-to-preview.cjs right 210 -50 -90 (side, size, bottom margin, side margin; a negative margin bleeds off the edge)`.