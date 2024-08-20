import { useEffect, useMemo, useRef, useState } from 'react';
import { MessageSkeleton } from '../../common/MessageSkeleton';

export interface NextMessagesObserverProps {
  onLoad: () => void;
  loading: boolean;
}

export function NextMessagesObserver(props: NextMessagesObserverProps) {
  const { onLoad, loading } = props;
  const [intersecting, setIntersecting] = useState(false);
  const ref = useRef<HTMLDivElement | null>(null);
  const timerRef = useRef<number>();

  const observer = useMemo(
    () =>
      new IntersectionObserver(([entry]) => {
        if (entry.isIntersecting) {
          timerRef.current = window.setTimeout(() => setIntersecting(true), 200);
        } else {
          clearTimeout(timerRef.current!);
        }
      }),
    [],
  );

  useEffect(() => {
    observer.observe(ref.current!);
    return () => observer.disconnect();
  }, [observer]);

  useEffect(() => {
    if (!intersecting) {
      return;
    }

    onLoad();

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [intersecting]);

  if (loading || intersecting) {
    return <MessageSkeleton count={4} />;
  }

  return <div className="wxchtf-next" ref={ref} />;
}
