import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '@/lib/api';
import { applicationsKeys } from './applications';
import type { InterviewDto, CreateInterviewDto, UpdateInterviewDto } from '@/types';

export const interviewsKeys = {
  all: ['interviews'] as const,
  byApplication: (applicationId: string) => [...interviewsKeys.all, 'application', applicationId] as const,
  details: () => [...interviewsKeys.all, 'detail'] as const,
  detail: (applicationId: string, interviewId: string) =>
    [...interviewsKeys.details(), applicationId, interviewId] as const,
};

// API Functions

export async function getInterviewById(
  applicationId: string,
  interviewId: string
): Promise<InterviewDto> {
  const { data } = await api.get<InterviewDto>(
    `/api/applications/${applicationId}/interviews/${interviewId}`
  );
  return data;
}

export async function createInterview(
  applicationId: string,
  dto: CreateInterviewDto
): Promise<InterviewDto> {
  const { data } = await api.post<InterviewDto>(
    `/api/applications/${applicationId}/interviews`,
    dto
  );
  return data;
}

export async function updateInterview(
  applicationId: string,
  interviewId: string,
  dto: UpdateInterviewDto
): Promise<InterviewDto> {
  const { data } = await api.put<InterviewDto>(
    `/api/applications/${applicationId}/interviews/${interviewId}`,
    dto
  );
  return data;
}

export async function deleteInterview(
  applicationId: string,
  interviewId: string
): Promise<void> {
  await api.delete(`/api/applications/${applicationId}/interviews/${interviewId}`);
}

// React Query Hooks

export function useInterview(applicationId?: string, interviewId?: string) {
  return useQuery({
    queryKey: interviewsKeys.detail(applicationId ?? '', interviewId ?? ''),
    queryFn: () => getInterviewById(applicationId!, interviewId!),
    enabled: Boolean(applicationId && interviewId),
  });
}

export function useCreateInterview() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({
      applicationId,
      dto,
    }: {
      applicationId: string;
      dto: CreateInterviewDto;
    }) => createInterview(applicationId, dto),
    onSuccess: (createdInterview, { applicationId }) => {
      queryClient.invalidateQueries({ queryKey: interviewsKeys.all });
      queryClient.invalidateQueries({ queryKey: applicationsKeys.detail(applicationId) });
      queryClient.invalidateQueries({ queryKey: applicationsKeys.all });
      queryClient.setQueryData(
        interviewsKeys.detail(applicationId, createdInterview.id),
        createdInterview
      );
    },
  });
}

export function useUpdateInterview() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({
      applicationId,
      interviewId,
      dto,
    }: {
      applicationId: string;
      interviewId: string;
      dto: UpdateInterviewDto;
    }) => updateInterview(applicationId, interviewId, dto),
    onSuccess: (updatedInterview, { applicationId }) => {
      queryClient.invalidateQueries({ queryKey: interviewsKeys.all });
      queryClient.invalidateQueries({ queryKey: applicationsKeys.detail(applicationId) });
      queryClient.invalidateQueries({ queryKey: applicationsKeys.all });
      queryClient.setQueryData(
        interviewsKeys.detail(applicationId, updatedInterview.id),
        updatedInterview
      );
    },
  });
}

export function useDeleteInterview() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({
      applicationId,
      interviewId,
    }: {
      applicationId: string;
      interviewId: string;
    }) => deleteInterview(applicationId, interviewId),
    onSuccess: (_, { applicationId }) => {
      queryClient.invalidateQueries({ queryKey: interviewsKeys.all });
      queryClient.invalidateQueries({ queryKey: applicationsKeys.detail(applicationId) });
      queryClient.invalidateQueries({ queryKey: applicationsKeys.all });
    },
  });
}
