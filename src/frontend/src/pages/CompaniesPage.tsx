import React, { useState, useMemo } from 'react';
import { useNavigate } from 'react-router-dom';
import { useCompanies } from '@/api/companies';
import CompanyCard from '@/components/companies/CompanyCard';
import CompanyFormModal from '@/components/companies/CompanyFormModal';
import CompanyDetailModal from '@/components/companies/CompanyDetailModal';
import type { CompanyDto } from '@/types';
import { Building2, Search, Plus } from 'lucide-react';

export const CompaniesPage: React.FC = () => {
  const navigate = useNavigate();
  const [includeArchived, setIncludeArchived] = useState(false);
  const [searchQuery, setSearchQuery] = useState('');

  // Modal states
  const [isCompanyFormOpen, setIsCompanyFormOpen] = useState(false);
  const [editingCompany, setEditingCompany] = useState<CompanyDto | null>(null);

  const [isCompanyDetailOpen, setIsCompanyDetailOpen] = useState(false);
  const [viewingCompany, setViewingCompany] = useState<CompanyDto | null>(null);
  const [detailModalInitialTab, setDetailModalInitialTab] = useState<'vacancies' | 'recruiters'>('vacancies');

  // Queries
  const { data: companies = [], isLoading, isError, refetch } = useCompanies(includeArchived);

  // Filtered companies based on search
  const filteredCompanies = useMemo(() => {
    if (!searchQuery.trim()) return companies;
    const query = searchQuery.toLowerCase();
    return companies.filter(
      (c) =>
        c.name.toLowerCase().includes(query) ||
        (c.industry && c.industry.toLowerCase().includes(query)) ||
        (c.notes && c.notes.toLowerCase().includes(query))
    );
  }, [companies, searchQuery]);

  // Modal actions
  const handleCreateCompany = () => {
    setEditingCompany(null);
    setIsCompanyFormOpen(true);
  };

  const handleEditCompany = (company: CompanyDto) => {
    setEditingCompany(company);
    setIsCompanyFormOpen(true);
  };

  const handleViewCompany = (company: CompanyDto) => {
    setViewingCompany(company);
    setDetailModalInitialTab('vacancies');
    setIsCompanyDetailOpen(true);
  };

  const handleViewCompanyRecruiters = (company: CompanyDto) => {
    setViewingCompany(company);
    setDetailModalInitialTab('recruiters');
    setIsCompanyDetailOpen(true);
  };

  const handleAddVacancy = (companyId: string) => {
    navigate(`/vacancies/new?companyId=${companyId}`);
  };

  const handleSelectVacancyFromDetail = (vacancyId: string) => {
    navigate(`/vacancies/${vacancyId}`);
  };

  return (
    <div className="space-y-4">
      {/* Header section */}
      <div className="flex flex-col gap-3 lg:flex-row lg:items-center lg:justify-between">
        <div>
          <h1 className="text-xl font-bold tracking-tight text-foreground">Companies</h1>
          <p className="text-sm text-muted-foreground">
            {isLoading
              ? 'Loading…'
              : `${companies.length} ${companies.length === 1 ? 'company' : 'companies'}${includeArchived ? ' (including archived)' : ''}`}
          </p>
        </div>

        <div className="flex flex-col gap-4 rounded-xl border border-border/60 bg-card p-2 shadow-sm sm:flex-row sm:items-center">
          <div className="relative sm:w-72">
            <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
            <input
              type="text"
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              placeholder="Search companies…"
              className="w-full rounded-lg border border-border py-2 pl-9 pr-3 text-sm shadow-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary"
            />
          </div>

          <label className="flex cursor-pointer items-center gap-2 text-xs font-semibold text-muted-foreground hover:text-foreground">
            <input
              type="checkbox"
              checked={includeArchived}
              onChange={(e) => setIncludeArchived(e.target.checked)}
              className="h-4 w-4 rounded border-border accent-primary"
            />
            Include archived
          </label>

          <button
            type="button"
            onClick={handleCreateCompany}
            className="inline-flex items-center justify-center gap-2 rounded-lg bg-primary px-3 py-2 text-sm font-semibold text-primary-foreground shadow-sm hover:bg-primary/90"
          >
            <Plus className="h-4 w-4" /> Add Company
          </button>

        </div>
      </div>

      <hr className="my-5 border-border/60" />

      {/* Content Section */}
      {
        isLoading ? (
          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
            {[1, 2, 3, 4, 5, 6].map((i) => (
              <div key={i} className="h-44 rounded-xl border border-border/60 bg-card p-5 animate-pulse space-y-3">
                <div className="h-5 w-1/2 bg-accent/60 rounded" />
                <div className="h-4 w-1/3 bg-accent/40 rounded" />
                <div className="h-10 w-full bg-accent/30 rounded mt-4" />
              </div>
            ))}
          </div>
        ) : isError ? (
          <div className="rounded-xl border border-destructive/30 bg-destructive/10 p-8 text-center">
            <p className="text-sm text-destructive font-medium mb-2">Failed to load companies directory.</p>
            <button
              type="button"
              onClick={() => refetch()}
              className="rounded-lg bg-destructive px-4 py-2 text-xs font-semibold text-destructive-foreground hover:bg-destructive/90"
            >
              Retry
            </button>
          </div>
        ) : filteredCompanies.length === 0 ? (
          <div className="rounded-xl border border-dashed border-border/80 bg-card p-12 text-center shadow-sm">
            <Building2 className="mx-auto h-12 w-12 text-muted-foreground/40 mb-3" />
            <h3 className="text-base font-semibold text-foreground">No companies found</h3>
            <p className="mt-1 text-sm text-muted-foreground max-w-sm mx-auto">
              {searchQuery
                ? 'No companies matched your search query. Try adjusting your filters.'
                : 'Start building your target employer directory by adding your first company.'}
            </p>
            <button
              type="button"
              onClick={handleCreateCompany}
              className="mt-4 inline-flex items-center gap-2 rounded-xl bg-primary px-4 py-2 text-xs font-semibold text-primary-foreground shadow-sm hover:bg-primary/90 transition-all"
            >
              <Plus className="h-4 w-4" /> Add First Company
            </button>
          </div>
        ) : (
          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
            {filteredCompanies.map((company) => (
              <CompanyCard
                key={company.id}
                company={company}
                onView={handleViewCompany}
                onEdit={handleEditCompany}
                onAddVacancy={handleAddVacancy}
                onViewRecruiters={handleViewCompanyRecruiters}
              />
            ))}
          </div>
        )
      }

      {/* Modals */}
      <CompanyFormModal
        isOpen={isCompanyFormOpen}
        onClose={() => setIsCompanyFormOpen(false)}
        companyToEdit={editingCompany}
      />

      <CompanyDetailModal
        isOpen={isCompanyDetailOpen}
        onClose={() => setIsCompanyDetailOpen(false)}
        company={viewingCompany}
        initialTab={detailModalInitialTab}
        onEditCompany={handleEditCompany}
        onAddVacancy={handleAddVacancy}
        onSelectVacancy={handleSelectVacancyFromDetail}
      />
    </div >
  );
};

export default CompaniesPage;
