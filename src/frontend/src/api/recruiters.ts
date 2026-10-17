import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '@/lib/api';
import { companiesKeys } from './companies';
import { applicationsKeys } from './applications';
import type { RecruiterDto, CreateRecruiterDto, UpdateRecruiterDto } from '@/types';

export const recruitersKeys = {
  all: ['recruiters'] as const,
  byCompany: (companyId: string) => [...recruitersKeys.all, 'company', companyId] as const,
  details: () => [...recruitersKeys.all, 'detail'] as const,
  detail: (companyId: string, recruiterId: string) =>
    [...recruitersKeys.details(), companyId, recruiterId] as const,
};

// API Functions

export async function getRecruiterById(
  companyId: string,
  recruiterId: string
): Promise<RecruiterDto> {
  const { data } = await api.get<RecruiterDto>(
    `/api/companies/${companyId}/recruiters/${recruiterId}`
  );
  return data;
}

export async function createRecruiter(
  companyId: string,
  dto: CreateRecruiterDto
): Promise<RecruiterDto> {
  const { data } = await api.post<RecruiterDto>(
    `/api/companies/${companyId}/recruiters`,
    dto
  );
  return data;
}

export async function updateRecruiter(
  companyId: string,
  recruiterId: string,
  dto: UpdateRecruiterDto
): Promise<RecruiterDto> {
  const { data } = await api.put<RecruiterDto>(
    `/api/companies/${companyId}/recruiters/${recruiterId}`,
    dto
  );
  return data;
}

export async function deleteRecruiter(
  companyId: string,
  recruiterId: string
): Promise<void> {
  await api.delete(`/api/companies/${companyId}/recruiters/${recruiterId}`);
}

// React Query Hooks

export function useRecruiter(companyId?: string, recruiterId?: string) {
  return useQuery({
    queryKey: recruitersKeys.detail(companyId ?? '', recruiterId ?? ''),
    queryFn: () => getRecruiterById(companyId!, recruiterId!),
    enabled: Boolean(companyId && recruiterId),
  });
}

export function useCreateRecruiter() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({
      companyId,
      dto,
    }: {
      companyId: string;
      dto: CreateRecruiterDto;
    }) => createRecruiter(companyId, dto),
    onSuccess: (createdRecruiter, { companyId }) => {
      queryClient.invalidateQueries({ queryKey: recruitersKeys.all });
      queryClient.invalidateQueries({ queryKey: companiesKeys.detail(companyId) });
      queryClient.invalidateQueries({ queryKey: companiesKeys.all });
      queryClient.invalidateQueries({ queryKey: applicationsKeys.all });
      queryClient.setQueryData(
        recruitersKeys.detail(companyId, createdRecruiter.id),
        createdRecruiter
      );
    },
  });
}

export function useUpdateRecruiter() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({
      companyId,
      recruiterId,
      dto,
    }: {
      companyId: string;
      recruiterId: string;
      dto: UpdateRecruiterDto;
    }) => updateRecruiter(companyId, recruiterId, dto),
    onSuccess: (updatedRecruiter, { companyId }) => {
      queryClient.invalidateQueries({ queryKey: recruitersKeys.all });
      queryClient.invalidateQueries({ queryKey: companiesKeys.detail(companyId) });
      queryClient.invalidateQueries({ queryKey: companiesKeys.all });
      queryClient.invalidateQueries({ queryKey: applicationsKeys.all });
      queryClient.setQueryData(
        recruitersKeys.detail(companyId, updatedRecruiter.id),
        updatedRecruiter
      );
    },
  });
}

export function useDeleteRecruiter() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({
      companyId,
      recruiterId,
    }: {
      companyId: string;
      recruiterId: string;
    }) => deleteRecruiter(companyId, recruiterId),
    onSuccess: (_, { companyId }) => {
      queryClient.invalidateQueries({ queryKey: recruitersKeys.all });
      queryClient.invalidateQueries({ queryKey: companiesKeys.detail(companyId) });
      queryClient.invalidateQueries({ queryKey: companiesKeys.all });
      queryClient.invalidateQueries({ queryKey: applicationsKeys.all });
    },
  });
}
