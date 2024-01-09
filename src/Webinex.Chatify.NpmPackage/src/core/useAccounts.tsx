import { useClient } from '../ChatifyContext';
import { useQuery } from './useQuery';

export function useAccounts() {
  const client = useClient();

  return useQuery({
    key: 'accounts',
    queryFn: () => client.accounts(),
  });
}
