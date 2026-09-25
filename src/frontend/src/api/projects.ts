import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '@/lib/api';
import type { ProjectDto, CreateProjectDto, UpdateProjectDto } from '@/types';

export const projectsKeys = {
  all: ['projects'] as const,
  lists: () => [...projectsKeys.all, 'list'] as const,
  list: (resumeRevisionId?: string) =>
    [...projectsKeys.lists(), { resumeRevisionId }] as const,
  details: () => [...projectsKeys.all, 'detail'] as const,
  detail: (id: string) => [...projectsKeys.details(), id] as const,
};

// API Functions

export async function getProjects(resumeRevisionId: string): Promise<ProjectDto[]> {
  const { data } = await api.get<ProjectDto[]>('/api/projects', {
    params: { resumeRevisionId },
  });
  return data;
}

export async function getProjectById(id: string): Promise<ProjectDto> {
  const { data } = await api.get<ProjectDto>(`/api/projects/${id}`);
  return data;
}

export async function createProject(dto: CreateProjectDto): Promise<ProjectDto> {
  const { data } = await api.post<ProjectDto>('/api/projects', dto);
  return data;
}

export async function updateProject(
  id: string,
  dto: UpdateProjectDto
): Promise<ProjectDto> {
  const { data } = await api.put<ProjectDto>(`/api/projects/${id}`, dto);
  return data;
}

export async function deleteProject(id: string): Promise<void> {
  await api.delete('/api/projects', {
    params: { id },
  });
}

// React Query Hooks

export function useProjects(resumeRevisionId?: string) {
  return useQuery({
    queryKey: projectsKeys.list(resumeRevisionId),
    queryFn: () => getProjects(resumeRevisionId!),
    enabled: Boolean(resumeRevisionId),
  });
}

export function useProject(id?: string) {
  return useQuery({
    queryKey: projectsKeys.detail(id ?? ''),
    queryFn: () => getProjectById(id!),
    enabled: Boolean(id),
  });
}

export function useCreateProject() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: createProject,
    onSuccess: (createdProject) => {
      queryClient.invalidateQueries({ queryKey: projectsKeys.all });
      queryClient.setQueryData(
        projectsKeys.detail(createdProject.id),
        createdProject
      );
    },
  });
}

export function useUpdateProject() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, dto }: { id: string; dto: UpdateProjectDto }) =>
      updateProject(id, dto),
    onSuccess: (updatedProject) => {
      queryClient.invalidateQueries({ queryKey: projectsKeys.all });
      queryClient.setQueryData(
        projectsKeys.detail(updatedProject.id),
        updatedProject
      );
    },
  });
}

export function useDeleteProject() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => deleteProject(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: projectsKeys.all });
    },
  });
}
