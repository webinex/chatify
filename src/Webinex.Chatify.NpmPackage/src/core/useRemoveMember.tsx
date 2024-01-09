import { useClient } from '../ChatifyContext';
import { RemoveMemberRequest } from './models';
import { useMutation } from './useMutation';

export function useRemoveMember() {
  const client = useClient();

  return useMutation({
    key: 'remove-member',
    queryFn: (args: RemoveMemberRequest) => client.removeMember(args),
  });
}
