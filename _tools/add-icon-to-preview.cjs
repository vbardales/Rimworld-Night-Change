// Cuts the ModIcon out of its source (flood fill from the border, so only the background goes
// transparent and the mascot's own near-black outline is untouched) and composites it into one
// corner of the delivered Preview, tilted +15deg in a left corner, -15deg in a right one
// (PUBLISHING.md, rule of 2026-09-29). Reads Art/Preview-without-icon.png, writes
// Art/ModIcon-cutout.png and Art/Preview-with-icon.png. Run with NODE_PATH set to a node_modules
// that holds sharp. Usage: node _tools/add-icon-to-preview.cjs [right|left] [size]
const sharp = require('sharp');
const path = require('path');
const root = path.resolve(__dirname, '..');
const side = process.argv[2] === 'left' ? 'left' : 'right';
const size = parseInt(process.argv[3] || '190', 10);
const angle = side === 'left' ? 15 : -15;

(async () => {
  const { data, info } = await sharp(path.join(root, 'Art/ModIcon-source.png')).ensureAlpha().raw().toBuffer({ resolveWithObject: true });
  const { width: w, height: h, channels: c } = info;
  const isBg = i => data[i] < 22 && data[i + 1] < 30 && data[i + 2] < 44;
  const seen = new Uint8Array(w * h);
  const stack = [];
  for (let x = 0; x < w; x++) { stack.push([x, 0], [x, h - 1]); }
  for (let y = 0; y < h; y++) { stack.push([0, y], [w - 1, y]); }
  let removed = 0;
  while (stack.length) {
    const [x, y] = stack.pop();
    if (x < 0 || y < 0 || x >= w || y >= h) continue;
    const p = y * w + x;
    if (seen[p] || !isBg(p * c)) continue;
    seen[p] = 1; data[p * c + 3] = 0; removed++;
    stack.push([x + 1, y], [x - 1, y], [x, y + 1], [x, y - 1]);
  }
  console.log(`cutout: removed ${removed} of ${w * h} pixels (${(100 * removed / (w * h)).toFixed(1)}%)`);
  await sharp(data, { raw: { width: w, height: h, channels: c } }).png({ compressionLevel: 9 })
    .toFile(path.join(root, 'Art/ModIcon-cutout.png'));

  const base = sharp(path.join(root, 'Art/Preview-without-icon.png'));
  const bm = await base.metadata();
  const icon = await sharp(path.join(root, 'Art/ModIcon-cutout.png'))
    .resize(size, size)
    .rotate(angle, { background: { r: 0, g: 0, b: 0, alpha: 0 } })
    .toBuffer();
  const im = await sharp(icon).metadata();
  const margin = parseInt(process.argv[4] || '-34', 10); // negative: bleeds off the corner
  const marginX = parseInt(process.argv[5] || String(margin), 10); // side margin, negative bleeds off the side
  const left = side === 'left' ? marginX : bm.width - im.width - marginX;
  const top = bm.height - im.height - margin;
  await base.composite([{ input: icon, left, top }]).png({ compressionLevel: 9 })
    .toFile(path.join(root, 'Art/Preview-with-icon.png'));
  console.log(`placed ${im.width}x${im.height} at ${left},${top} (${side}, ${angle}deg)`);
})();
