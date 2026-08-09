import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '@/lib/api';
import type {
  ResumeRevisionDto,
  CreateResumeDraftDto,
  UpdateResumeDraftDto,
} from '@/types';

export const resumesKeys = {
  all: ['resumes'] as const,
  lists: () => [...resumesKeys.all, 'list'] as const,
  details: () => [...resumesKeys.all, 'detail'] as const,
  detail: (id: string) => [...resumesKeys.details(), id] as const,
};

export async function getResumeRevisions(): Promise<ResumeRevisionDto[]> {
  const { data } = await api.get<ResumeRevisionDto[]>('/api/resumes');
  return data;
}

export async function getResumeRevisionById(id: string): Promise<ResumeRevisionDto> {
  const { data } = await api.get<ResumeRevisionDto>(`/api/resumes/${id}`);
  return data;
}

export async function createResumeDraft(dto: CreateResumeDraftDto): Promise<ResumeRevisionDto> {
  const { data } = await api.post<ResumeRevisionDto>('/api/resumes', dto);
  return data;
}

export async function updateResumeDraft(
  id: string,
  dto: UpdateResumeDraftDto
): Promise<ResumeRevisionDto> {
  const { data } = await api.put<ResumeRevisionDto>(`/api/resumes/${id}`, dto);
  return data;
}

export async function spawnResumeVersion(id: string): Promise<ResumeRevisionDto> {
  const { data } = await api.post<ResumeRevisionDto>(`/api/resumes/${id}/spawn`);
  return data;
}

// React Query Hooks

export function useResumeRevisions() {
  return useQuery({
    queryKey: resumesKeys.lists(),
    queryFn: getResumeRevisions,
  });
}

export function useResumeRevision(id?: string) {
  return useQuery({
    queryKey: resumesKeys.detail(id ?? ''),
    queryFn: () => getResumeRevisionById(id!),
    enabled: Boolean(id),
  });
}

export function useCreateResumeDraft() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: createResumeDraft,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: resumesKeys.all });
    },
  });
}

export function useUpdateResumeDraft() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, dto }: { id: string; dto: UpdateResumeDraftDto }) =>
      updateResumeDraft(id, dto),
    onSuccess: (updatedRevision) => {
      queryClient.invalidateQueries({ queryKey: resumesKeys.all });
      queryClient.setQueryData(resumesKeys.detail(updatedRevision.id), updatedRevision);
    },
  });
}

export function useSpawnResumeVersion() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => spawnResumeVersion(id),
    onSuccess: (spawnedRevision) => {
      queryClient.invalidateQueries({ queryKey: resumesKeys.all });
      queryClient.setQueryData(resumesKeys.detail(spawnedRevision.id), spawnedRevision);
    },
  });
}
