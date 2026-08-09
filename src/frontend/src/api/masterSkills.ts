import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '@/lib/api';
import type { MasterSkillDto, CreateMasterSkillDto, SkillCategory } from '@/types';

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

export async function getMasterSkillById(id: string): Promise<MasterSkillDto> {
  const { data } = await api.get<MasterSkillDto>(`/api/master-skills/${id}`);
  return data;
}

export async function createMasterSkill(dto: CreateMasterSkillDto): Promise<MasterSkillDto> {
  const { data } = await api.post<MasterSkillDto>('/api/master-skills', dto);
  return data;
}

export function useMasterSkills(category?: SkillCategory, includeInactive = false) {
  return useQuery({
    queryKey: masterSkillsKeys.list(category, includeInactive),
    queryFn: () => getMasterSkills(category, includeInactive),
  });
}

export function useMasterSkill(id?: string) {
  return useQuery({
    queryKey: masterSkillsKeys.detail(id ?? ''),
    queryFn: () => getMasterSkillById(id!),
    enabled: Boolean(id),
  });
}

export function useCreateMasterSkill() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: createMasterSkill,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: masterSkillsKeys.all });
    },
  });
}
