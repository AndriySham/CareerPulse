import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '@/lib/api';
import { applicationsKeys } from './applications';
import type {
  ApplicationCommunicationDto,
  CreateApplicationCommunicationDto,
  UpdateApplicationCommunicationDto,
} from '@/types';

export const communicationsKeys = {
  all: ['communications'] as const,
  byApplication: (applicationId: string) =>
    [...communicationsKeys.all, 'application', applicationId] as const,
  details: () => [...communicationsKeys.all, 'detail'] as const,
  detail: (applicationId: string, communicationId: string) =>
    [...communicationsKeys.details(), applicationId, communicationId] as const,
};

// API Functions

export async function getCommunicationById(
  applicationId: string,
  communicationId: string
): Promise<ApplicationCommunicationDto> {
  const { data } = await api.get<ApplicationCommunicationDto>(
    `/api/applications/${applicationId}/communications/${communicationId}`
  );
  return data;
}

export async function createCommunication(
  applicationId: string,
  dto: CreateApplicationCommunicationDto
): Promise<ApplicationCommunicationDto> {
  const { data } = await api.post<ApplicationCommunicationDto>(
    `/api/applications/${applicationId}/communications`,
    dto
  );
  return data;
}

export async function updateCommunication(
  applicationId: string,
  communicationId: string,
  dto: UpdateApplicationCommunicationDto
): Promise<ApplicationCommunicationDto> {
  const { data } = await api.put<ApplicationCommunicationDto>(
    `/api/applications/${applicationId}/communications/${communicationId}`,
    dto
  );
  return data;
}

export async function deleteCommunication(
  applicationId: string,
  communicationId: string
): Promise<void> {
  await api.delete(
    `/api/applications/${applicationId}/communications/${communicationId}`
  );
}

// React Query Hooks

export function useCommunication(applicationId?: string, communicationId?: string) {
  return useQuery({
    queryKey: communicationsKeys.detail(applicationId ?? '', communicationId ?? ''),
    queryFn: () => getCommunicationById(applicationId!, communicationId!),
    enabled: Boolean(applicationId && communicationId),
  });
}

export function useCreateCommunication() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({
      applicationId,
      dto,
    }: {
      applicationId: string;
      dto: CreateApplicationCommunicationDto;
    }) => createCommunication(applicationId, dto),
    onSuccess: (createdCommunication, { applicationId }) => {
      queryClient.invalidateQueries({ queryKey: communicationsKeys.all });
      queryClient.invalidateQueries({ queryKey: applicationsKeys.detail(applicationId) });
      queryClient.invalidateQueries({ queryKey: applicationsKeys.all });
      queryClient.setQueryData(
        communicationsKeys.detail(applicationId, createdCommunication.id),
        createdCommunication
      );
    },
  });
}

export function useUpdateCommunication() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({
      applicationId,
      communicationId,
      dto,
    }: {
      applicationId: string;
      communicationId: string;
      dto: UpdateApplicationCommunicationDto;
    }) => updateCommunication(applicationId, communicationId, dto),
    onSuccess: (updatedCommunication, { applicationId }) => {
      queryClient.invalidateQueries({ queryKey: communicationsKeys.all });
      queryClient.invalidateQueries({ queryKey: applicationsKeys.detail(applicationId) });
      queryClient.invalidateQueries({ queryKey: applicationsKeys.all });
      queryClient.setQueryData(
        communicationsKeys.detail(applicationId, updatedCommunication.id),
        updatedCommunication
      );
    },
  });
}

export function useDeleteCommunication() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({
      applicationId,
      communicationId,
    }: {
      applicationId: string;
      communicationId: string;
    }) => deleteCommunication(applicationId, communicationId),
    onSuccess: (_, { applicationId }) => {
      queryClient.invalidateQueries({ queryKey: communicationsKeys.all });
      queryClient.invalidateQueries({ queryKey: applicationsKeys.detail(applicationId) });
      queryClient.invalidateQueries({ queryKey: applicationsKeys.all });
    },
  });
}
