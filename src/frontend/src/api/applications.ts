import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '@/lib/api';
import { resumesKeys } from './resumes';
import type {
  ApplicationDto,
  SubmitApplicationDto,
  ChangeApplicationStatusDto,
  ApplicationStatus,
} from '@/types';

export const applicationsKeys = {
  all: ['applications'] as const,
  lists: () => [...applicationsKeys.all, 'list'] as const,
  list: (filters?: { status?: ApplicationStatus; vacancyId?: string; companyId?: string }) =>
    [...applicationsKeys.lists(), filters] as const,
  details: () => [...applicationsKeys.all, 'detail'] as const,
  detail: (id: string) => [...applicationsKeys.details(), id] as const,
};

export async function getApplications(filters?: {
  status?: ApplicationStatus;
  vacancyId?: string;
  companyId?: string;
}): Promise<ApplicationDto[]> {
  const { data } = await api.get<ApplicationDto[]>('/api/applications', {
    params: filters,
  });
  return data;
}

export async function getApplicationById(id: string): Promise<ApplicationDto> {
  const { data } = await api.get<ApplicationDto>(`/api/applications/${id}`);
  return data;
}

export async function submitApplication(dto: SubmitApplicationDto): Promise<ApplicationDto> {
  const { data } = await api.post<ApplicationDto>('/api/applications', dto);
  return data;
}

export async function changeApplicationStatus(
  id: string,
  dto: ChangeApplicationStatusDto
): Promise<ApplicationDto> {
  const { data } = await api.put<ApplicationDto>(`/api/applications/${id}/status`, dto);
  return data;
}

// React Query Hooks

export function useApplications(filters?: {
  status?: ApplicationStatus;
  vacancyId?: string;
  companyId?: string;
}) {
  return useQuery({
    queryKey: applicationsKeys.list(filters),
    queryFn: () => getApplications(filters),
  });
}

export function useApplication(id?: string) {
  return useQuery({
    queryKey: applicationsKeys.detail(id ?? ''),
    queryFn: () => getApplicationById(id!),
    enabled: Boolean(id),
  });
}

export function useSubmitApplication() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: submitApplication,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: applicationsKeys.all });
      queryClient.invalidateQueries({ queryKey: resumesKeys.all });
    },
  });
}

export function useChangeApplicationStatus() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, dto }: { id: string; dto: ChangeApplicationStatusDto }) =>
      changeApplicationStatus(id, dto),
    onSuccess: (updatedApp) => {
      queryClient.invalidateQueries({ queryKey: applicationsKeys.all });
      queryClient.invalidateQueries({ queryKey: resumesKeys.all });
      queryClient.setQueryData(applicationsKeys.detail(updatedApp.id), updatedApp);
    },
  });
}
