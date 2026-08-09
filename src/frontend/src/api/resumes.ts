import { useQuery } from '@tanstack/react-query';
import { api } from '@/lib/api';
import type { ResumeRevisionDto } from '@/types';

export const resumesKeys = {
  all: ['resumes'] as const,
  lists: () => [...resumesKeys.all, 'list'] as const,
};

export async function getResumeRevisions(): Promise<ResumeRevisionDto[]> {
  const { data } = await api.get<ResumeRevisionDto[]>('/api/resumes');
  return data;
}

export function useResumeRevisions() {
  return useQuery({
    queryKey: resumesKeys.lists(),
    queryFn: getResumeRevisions,
  });
}
