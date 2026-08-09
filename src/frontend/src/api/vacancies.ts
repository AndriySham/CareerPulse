import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '@/lib/api';
import type { VacancyDto, CreateVacancyDto, UpdateVacancyDto } from '@/types';

export const vacanciesKeys = {
  all: ['vacancies'] as const,
  lists: () => [...vacanciesKeys.all, 'list'] as const,
  list: (companyId?: string) => [...vacanciesKeys.lists(), { companyId }] as const,
  details: () => [...vacanciesKeys.all, 'detail'] as const,
  detail: (id: string) => [...vacanciesKeys.details(), id] as const,
};

export async function getVacancies(companyId?: string): Promise<VacancyDto[]> {
  const { data } = await api.get<VacancyDto[]>('/api/vacancies', {
    params: companyId ? { companyId } : undefined,
  });
  return data;
}

export async function getVacancyById(id: string): Promise<VacancyDto> {
  const { data } = await api.get<VacancyDto>(`/api/vacancies/${id}`);
  return data;
}

export async function createVacancy(dto: CreateVacancyDto): Promise<VacancyDto> {
  const { data } = await api.post<VacancyDto>('/api/vacancies', dto);
  return data;
}

export async function updateVacancy(id: string, dto: UpdateVacancyDto): Promise<VacancyDto> {
  const { data } = await api.put<VacancyDto>(`/api/vacancies/${id}`, dto);
  return data;
}

// React Query Hooks

export function useVacancies(companyId?: string, enabled = true) {
  return useQuery({
    queryKey: vacanciesKeys.list(companyId),
    queryFn: () => getVacancies(companyId),
    enabled: enabled && (companyId === undefined || Boolean(companyId)),
  });
}

export function useVacancy(id?: string) {
  return useQuery({
    queryKey: vacanciesKeys.detail(id ?? ''),
    queryFn: () => getVacancyById(id!),
    enabled: Boolean(id),
  });
}

export function useCreateVacancy() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: createVacancy,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: vacanciesKeys.all });
    },
  });
}

export function useUpdateVacancy() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, dto }: { id: string; dto: UpdateVacancyDto }) => updateVacancy(id, dto),
    onSuccess: (updatedVacancy) => {
      queryClient.invalidateQueries({ queryKey: vacanciesKeys.all });
      queryClient.setQueryData(vacanciesKeys.detail(updatedVacancy.id), updatedVacancy);
    },
  });
}
