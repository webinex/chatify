export function uniqBy<T, TKey extends string | number | symbol>(items: T[], selector: (x: T) => TKey) {
  const keys = {} as Record<TKey, null>;
  const result: T[] = [];

  items.forEach((x) => {
    const key = selector(x);

    if (keys.hasOwnProperty(key)) {
      return;
    }

    keys[key] = null;
    result.push(x);
  });

  return result;
}
