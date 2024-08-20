import { useLocalizer } from './localizer';

export function useFormatter() {
  const localizer = useLocalizer();

  return {
    size: (value: number) => {
      if (value < 1024) {
        return localizer.size.bytes(value);
      } else if (value < 1024 * 1024) {
        return localizer.size.kb(value / 1024);
      } else {
        return localizer.size.mb(value / 1024 / 1024);
      }
    },
  };
}
