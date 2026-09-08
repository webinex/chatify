import { useEffect, useRef, useState } from 'react';

export function useCompact(thresholdOrCompact: number | boolean = 768) {
  const ref = useRef<HTMLDivElement>(null);
  const [size, setSize] = useState<{ width: number; height: number }>();
  const automatic = typeof thresholdOrCompact === 'number';

  useEffect(() => {
    if (!automatic) {
      setSize(undefined);
      return;
    }

    const el = ref.current;
    if (!el) return;

    const observer = new ResizeObserver(() => {
      const { width, height } = el.getBoundingClientRect();
      setSize({ width, height });
    });

    observer.observe(el);
    return () => observer.disconnect();
  }, [automatic]);

  if (typeof thresholdOrCompact === 'boolean') {
    return [ref, thresholdOrCompact, size] as const;
  }

  const compact = size != null ? size.width < thresholdOrCompact : undefined;
  return [ref, compact, size] as const;
}
