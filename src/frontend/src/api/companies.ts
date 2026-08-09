import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '@/lib/api';
import type { CompanyDto, CreateCompanyDto, UpdateCompanyDto } from '@/types';

export const companiesKeys = {
  all: ['companies'] as const,
  lists: () => [...companiesKeys.all, 'list'] as const,
  list: (includeArchived: boolean) => [...companiesKeys.lists(), { includeArchived }] as const,
  details: () => [...companiesKeys.all, 'detail'] as const,
  detail: (id: string) => [...companiesKeys.details(), id] as const,
};

export async function getCompanies(includeArchived = false): Promise<CompanyDto[]> {
  const { data } = await api.get<CompanyDto[]>('/api/companies', {
    params: { includeArchived },
  });
  return data;
}

export async function getCompanyById(id: string): Promise<CompanyDto> {
  const { data } = await api.get<CompanyDto>(`/api/companies/${id}`);
  return data;
}

export async function createCompany(dto: CreateCompanyDto): Promise<CompanyDto> {
  const { data } = await api.post<CompanyDto>('/api/companies', dto);
  return data;
}

export async function updateCompany(id: string, dto: UpdateCompanyDto): Promise<CompanyDto> {
  const { data } = await api.put<CompanyDto>(`/api/companies/${id}`, dto);
  return data;
}

// React Query Hooks

export function useCompanies(includeArchived = false) {
  return useQuery({
    queryKey: companiesKeys.list(includeArchived),
    queryFn: () => getCompanies(includeArchived),
  });
}

export function useCompany(id?: string) {
  return useQuery({
    queryKey: companiesKeys.detail(id ?? ''),
    queryFn: () => getCompanyById(id!),
    enabled: Boolean(id),
  });
}

export function useCreateCompany() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: createCompany,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: companiesKeys.all });
    },
  });
}

export function useUpdateCompany() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, dto }: { id: string; dto: UpdateCompanyDto }) => updateCompany(id, dto),
    onSuccess: (updatedCompany) => {
      queryClient.invalidateQueries({ queryKey: companiesKeys.all });
      queryClient.setQueryData(companiesKeys.detail(updatedCompany.id), updatedCompany);
    },
  });
}
