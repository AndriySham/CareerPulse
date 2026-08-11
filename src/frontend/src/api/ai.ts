import { useMutation } from '@tanstack/react-query';
import { api } from '@/lib/api';
import type {
  ResumeImportResultDto,
  SkillResolutionDecisionDto,
} from '@/types';

export const aiKeys = {
  all: ['ai'] as const,
  import: () => [...aiKeys.all, 'import'] as const,
  resolveSkills: () => [...aiKeys.all, 'resolve-skills'] as const,
};

export async function importResumePdf(file: File): Promise<ResumeImportResultDto> {
  const formData = new FormData();
  formData.append('file', file);

  const { data } = await api.post<ResumeImportResultDto>('/api/ai/import-resume', formData, {
    headers: {
      'Content-Type': 'multipart/form-data',
    },
  });
  return data;
}

export async function resolveSkills(
  decisions: SkillResolutionDecisionDto[]
): Promise<ResumeImportResultDto> {
  const { data } = await api.post<ResumeImportResultDto>('/api/ai/resolve-skills', decisions);
  return data;
}

// React Query Hooks

export function useImportResumePdf() {
  return useMutation({
    mutationFn: (file: File) => importResumePdf(file),
  });
}

export function useResolveSkills() {
  return useMutation({
    mutationFn: (decisions: SkillResolutionDecisionDto[]) => resolveSkills(decisions),
  });
}
