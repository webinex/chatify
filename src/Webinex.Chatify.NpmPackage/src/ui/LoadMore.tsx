import { useEffect, useMemo, useRef, useState } from 'react';
import { useLoadMore } from '../core';
import { MessageSkeleton } from './MessageSkeleton';

export interface LoadMoreProps {
  chatId: string;
}

export function LoadMore(props: LoadMoreProps) {
  const { chatId } = props;
  const [loadMore, { isFetching }] = useLoadMore({ chatId });
  const [intersecting, setIntersecting] = useState(false);
  const ref = useRef<HTMLDivElement | null>(null);

  const observer = useMemo(
    () => new IntersectionObserver(([entry]) => setIntersecting((prev) => prev || entry.isIntersecting)),
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

    loadMore();

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [intersecting]);

  if (isFetching) {
    return <MessageSkeleton count={4} />;
  }

  return <div ref={ref} />;
}
