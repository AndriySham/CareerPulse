import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '@/lib/api';
import type { EducationDto, CreateEducationDto, UpdateEducationDto } from '@/types';

export const educationsKeys = {
  all: ['educations'] as const,
  lists: () => [...educationsKeys.all, 'list'] as const,
  list: (resumeRevisionId?: string) =>
    [...educationsKeys.lists(), { resumeRevisionId }] as const,
  details: () => [...educationsKeys.all, 'detail'] as const,
  detail: (id: string) => [...educationsKeys.details(), id] as const,
};

// API Functions

export async function getEducations(resumeRevisionId: string): Promise<EducationDto[]> {
  const { data } = await api.get<EducationDto[]>('/api/educations', {
    params: { resumeRevisionId },
  });
  return data;
}

export async function getEducationById(id: string): Promise<EducationDto> {
  const { data } = await api.get<EducationDto>(`/api/educations/${id}`);
  return data;
}

export async function createEducation(dto: CreateEducationDto): Promise<EducationDto> {
  const { data } = await api.post<EducationDto>('/api/educations', dto);
  return data;
}

export async function updateEducation(
  id: string,
  dto: UpdateEducationDto
): Promise<EducationDto> {
  const { data } = await api.put<EducationDto>(`/api/educations/${id}`, dto);
  return data;
}

export async function deleteEducation(id: string): Promise<void> {
  await api.delete('/api/educations', {
    params: { id },
  });
}

// React Query Hooks

export function useEducations(resumeRevisionId?: string) {
  return useQuery({
    queryKey: educationsKeys.list(resumeRevisionId),
    queryFn: () => getEducations(resumeRevisionId!),
    enabled: Boolean(resumeRevisionId),
  });
}

export function useEducation(id?: string) {
  return useQuery({
    queryKey: educationsKeys.detail(id ?? ''),
    queryFn: () => getEducationById(id!),
    enabled: Boolean(id),
  });
}

export function useCreateEducation() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: createEducation,
    onSuccess: (createdEducation) => {
      queryClient.invalidateQueries({ queryKey: educationsKeys.all });
      queryClient.setQueryData(
        educationsKeys.detail(createdEducation.id),
        createdEducation
      );
    },
  });
}

export function useUpdateEducation() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, dto }: { id: string; dto: UpdateEducationDto }) =>
      updateEducation(id, dto),
    onSuccess: (updatedEducation) => {
      queryClient.invalidateQueries({ queryKey: educationsKeys.all });
      queryClient.setQueryData(
        educationsKeys.detail(updatedEducation.id),
        updatedEducation
      );
    },
  });
}

export function useDeleteEducation() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => deleteEducation(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: educationsKeys.all });
    },
  });
}
