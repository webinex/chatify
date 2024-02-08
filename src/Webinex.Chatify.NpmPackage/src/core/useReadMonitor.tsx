import { useEffect, useRef } from 'react';
import { useClient } from '../ChatifyContext';
import { useDispatch, useSelect } from './reducer';

export function useReadMonitor() {
  const client = useClient();
  const [queued, timestamp] = useSelect((x) => [x.queue.read.queued, x.queue.read.timestamp], []);
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
      for (const id of queued) {
        dispatch({ type: 'read_send', data: { id } });

        client
          .read({ id })
          .catch(() => dispatch({ type: 'read_reject', data: { id, timestamp: new Date().getTime() } }));
      }
    }, 100);

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [timestamp]);
}
