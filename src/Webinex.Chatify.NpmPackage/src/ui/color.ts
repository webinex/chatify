interface ColorOptions {
  saturation?: number;
  lightness?: number;
}

const DEFAULT_COLOR_OPTIONS: ColorOptions = {
  saturation: 100,
  lightness: 75,
};

export function color(value: string, options: ColorOptions = {}) {
  options = Object.assign({}, DEFAULT_COLOR_OPTIONS, options);

  let hash = 0;
  for (let i = 0; i < value.length; i++) {
    hash = value.charCodeAt(i) + ((hash << 5) - hash);
    hash = hash & hash;
  }
  return `hsl(${hash % 360}, ${options.saturation}%, ${options.lightness}%)`;
}
