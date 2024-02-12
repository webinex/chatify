import { useEffect, useMemo, useRef, useState } from 'react';
import { useFetchNextMutation } from '../core';
import { MessageSkeleton } from './MessageSkeleton';

export interface NextProps {
  chatId: string;
}

export function Next(props: NextProps) {
  const { chatId } = props;
  const [fetchNext, { isFetching }] = useFetchNextMutation({ chatId });
  const [intersecting, setIntersecting] = useState(false);
  const ref = useRef<HTMLDivElement | null>(null);
  const timerRef = useRef<NodeJS.Timer>();

  const observer = useMemo(
    () =>
      new IntersectionObserver(([entry]) => {
        if (entry.isIntersecting) {
          timerRef.current = setTimeout(() => setIntersecting(true), 200);
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

    fetchNext();

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [intersecting]);

  if (isFetching || intersecting) {
    return <MessageSkeleton count={4} />;
  }

  return <div className="wxchtf-next" ref={ref}></div>;
}
