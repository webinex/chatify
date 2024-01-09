import { useEffect, useRef } from 'react';
import { useClient } from '../ChatifyContext';
import { useDispatch, useSelect } from './reducer';

export function useReadMonitor() {
  const client = useClient();
  const [queued, timestamp] = useSelect((x) => [x.read.queued, x.read.timestamp], []);
  const dispatch = useDispatch();
  const timer = useRef<NodeJS.Timeout | null>(null);

  useEffect(() => {
    if (queued.length === 0) {
      return;
    }

    if (timer.current) {
      clearTimeout(timer.current);
    }

    timer.current = setTimeout(() => {
      dispatch({ type: 'read_send', data: { ids: queued } });
      client
        .read({ ids: queued })
        .catch(() =>
          dispatch({ type: 'read_reject', data: { ids: queued, timestamp: new Date().getTime() } }),
        );
    }, 100);

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [timestamp]);
}
