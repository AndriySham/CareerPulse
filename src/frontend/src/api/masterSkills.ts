import { useQuery } from '@tanstack/react-query';
import { api } from '@/lib/api';
import type { MasterSkillDto, SkillCategory } from '@/types';

export const masterSkillsKeys = {
  all: ['masterSkills'] as const,
  lists: () => [...masterSkillsKeys.all, 'list'] as const,
  list: (category?: SkillCategory, includeInactive = false) =>
    [...masterSkillsKeys.lists(), { category, includeInactive }] as const,
  details: () => [...masterSkillsKeys.all, 'detail'] as const,
  detail: (id: string) => [...masterSkillsKeys.details(), id] as const,
};

export async function getMasterSkills(
  category?: SkillCategory,
  includeInactive = false
): Promise<MasterSkillDto[]> {
  const { data } = await api.get<MasterSkillDto[]>('/api/master-skills', {
    params: { category, includeInactive },
  });
  return data;
}

export function useMasterSkills(category?: SkillCategory, includeInactive = false) {
  return useQuery({
    queryKey: masterSkillsKeys.list(category, includeInactive),
    queryFn: () => getMasterSkills(category, includeInactive),
  });
}
