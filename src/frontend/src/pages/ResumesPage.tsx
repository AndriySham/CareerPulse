import React, { useState, useMemo } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  useResumeRevisions,
  useSpawnResumeVersion,
} from '@/api/resumes';
import ResumeCard from '@/components/resumes/ResumeCard';
import ResumeStatusBadge from '@/components/resumes/ResumeStatusBadge';
import ResumeDetailModal from '@/components/resumes/ResumeDetailModal';
import ResumeVersionHistoryModal from '@/components/resumes/ResumeVersionHistoryModal';
import CustomSelect from '@/components/ui/CustomSelect';
import ErrorAlert from '@/components/ui/ErrorAlert';
import type { ResumeRevisionDto } from '@/types';
import {
  FileUser,
  Plus,
  Search,
  SlidersHorizontal,
  LayoutGrid,
  List,
  Edit3,
  Lock,
  GitBranch,
  FileText,
  User,
  Award,
} from 'lucide-react';

export const ResumesPage: React.FC = () => {
  const navigate = useNavigate();
  const [viewMode, setViewMode] = useState<'grid' | 'list'>('grid');
  const [searchQuery, setSearchQuery] = useState('');
  const [statusFilter, setStatusFilter] = useState<string>('');

  // Modal States
  const [isDetailModalOpen, setIsDetailModalOpen] = useState(false);
  const [isHistoryModalOpen, setIsHistoryModalOpen] = useState(false);
  const [selectedRevision, setSelectedRevision] = useState<ResumeRevisionDto | null>(null);
  const [spawnError, setSpawnError] = useState<unknown | null>(null);

  // Queries & Mutations
  const { data: revisions = [], isLoading, isError, refetch } = useResumeRevisions();
  const spawnMutation = useSpawnResumeVersion();

  // Statistics
  const totalRevisions = revisions.length;
  const draftCount = useMemo(
    () => revisions.filter((r) => r.status === 'Draft').length,
    [revisions]
  );
  const appliedCount = useMemo(
    () => revisions.filter((r) => r.status === 'Applied').length,
    [revisions]
  );
  const maxVersion = useMemo(
    () => (revisions.length > 0 ? Math.max(...revisions.map((r) => r.version)) : 0),
    [revisions]
  );

  // Filtered revisions
  const filteredRevisions = useMemo(() => {
    return revisions.filter((r) => {
      // Status filter
      if (statusFilter && r.status !== statusFilter) {
        return false;
      }
      // Search filter
      if (searchQuery.trim()) {
        const query = searchQuery.toLowerCase();
        const summaryMatch = r.professionalSummary?.toLowerCase().includes(query);
        const nameMatch = r.personalInfo?.fullName?.toLowerCase().includes(query);
        const emailMatch = r.personalInfo?.email?.toLowerCase().includes(query);
        const locationMatch = r.personalInfo?.location?.toLowerCase().includes(query);
        const skillMatch = r.skills?.some((s) => s.skillName?.toLowerCase().includes(query));

        return summaryMatch || nameMatch || emailMatch || locationMatch || skillMatch;
      }
      return true;
    });
  }, [revisions, statusFilter, searchQuery]);

  // Handlers
  const handleOpenCreate = () => {
    navigate('/resumes/new');
  };

  const handleOpenEdit = (revision: ResumeRevisionDto) => {
    navigate(`/resumes/${revision.id}`);
  };


  const handleOpenDetail = (revision: ResumeRevisionDto) => {
    setSelectedRevision(revision);
    setIsDetailModalOpen(true);
  };

  const handleOpenHistory = (revision: ResumeRevisionDto) => {
    setSelectedRevision(revision);
    setIsHistoryModalOpen(true);
  };

  const handleSpawnVersion = (revision: ResumeRevisionDto) => {
    if (revision.status !== 'Applied') return;
    setSpawnError(null);
    spawnMutation.mutate(revision.id, {
      onSuccess: (newRevision) => {
        // Automatically open the new draft in edit mode or detail mode
        setSelectedRevision(newRevision);
        setIsDetailModalOpen(true);
      },
      onError: (err) => {
        setSpawnError(err);
      },
    });
  };

  return (
    <div className="space-y-6">
      {/* Top Header Section */}
      <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between border-b border-border/40 pb-5">
        <div className="flex items-center gap-3">
          <div className="p-2.5 rounded-xl bg-primary/10 text-primary border border-primary/20">
            <FileUser className="h-6 w-6" />
          </div>
          <div>
            <h1 className="text-2xl font-bold tracking-tight text-foreground">
              Resumes & Revision Snapshots
            </h1>
            <p className="text-sm text-muted-foreground">
              Manage tailored resume profiles, version history, and ADR 005 Copy-on-Write drafts.
            </p>
          </div>
        </div>

        <button
          type="button"
          onClick={handleOpenCreate}
          className="inline-flex items-center gap-2 rounded-xl bg-primary px-4 py-2.5 text-sm font-semibold text-primary-foreground shadow-sm hover:bg-primary/90 transition-all cursor-pointer"
        >
          <Plus className="h-4 w-4" /> Create Draft Resume
        </button>
      </div>

      {/* Spawn Error Notification */}
      <ErrorAlert error={spawnError} onDismiss={() => setSpawnError(null)} />

      {/* Summary KPI Cards */}
      <div className="grid grid-cols-1 gap-4 sm:grid-cols-4">
        <div className="rounded-xl border border-border/60 bg-card p-4 shadow-sm flex items-center gap-4">
          <div className="p-3 rounded-lg bg-primary/10 text-primary">
            <FileText className="h-5 w-5" />
          </div>
          <div>
            <div className="text-2xl font-bold text-foreground">{totalRevisions}</div>
            <div className="text-xs text-muted-foreground font-medium">Total Revisions</div>
          </div>
        </div>

        <div className="rounded-xl border border-border/60 bg-card p-4 shadow-sm flex items-center gap-4">
          <div className="p-3 rounded-lg bg-amber-500/10 text-amber-400">
            <Edit3 className="h-5 w-5" />
          </div>
          <div>
            <div className="text-2xl font-bold text-foreground">{draftCount}</div>
            <div className="text-xs text-muted-foreground font-medium">Editable Drafts</div>
          </div>
        </div>

        <div className="rounded-xl border border-border/60 bg-card p-4 shadow-sm flex items-center gap-4">
          <div className="p-3 rounded-lg bg-emerald-500/10 text-emerald-400">
            <Lock className="h-5 w-5" />
          </div>
          <div>
            <div className="text-2xl font-bold text-foreground">{appliedCount}</div>
            <div className="text-xs text-muted-foreground font-medium">Applied (Immutable)</div>
          </div>
        </div>

        <div className="rounded-xl border border-border/60 bg-card p-4 shadow-sm flex items-center gap-4">
          <div className="p-3 rounded-lg bg-purple-500/10 text-purple-400">
            <GitBranch className="h-5 w-5" />
          </div>
          <div>
            <div className="text-2xl font-bold text-foreground">v{maxVersion}</div>
            <div className="text-xs text-muted-foreground font-medium">Highest Version</div>
          </div>
        </div>
      </div>

      {/* Filter and Control Toolbar */}
      <div className="flex flex-col gap-3 lg:flex-row lg:items-center lg:justify-between rounded-xl border border-border/60 bg-card p-4 shadow-sm">
        {/* Search Input */}
        <div className="relative flex-1 max-w-md">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
          <input
            type="text"
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            placeholder="Search by summary, developer name, email, or skills..."
            className="w-full rounded-lg border border-border bg-background pl-9 pr-4 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary"
          />
        </div>

        {/* Filters & View Toggle */}
        <div className="flex flex-wrap items-center gap-3">
          <div className="flex items-center gap-1.5 text-xs text-muted-foreground font-medium">
            <SlidersHorizontal className="h-3.5 w-3.5" />
            <span>Filter:</span>
          </div>

          <CustomSelect
            value={statusFilter}
            onChange={(val) => setStatusFilter(val)}
            options={[
              { value: '', label: 'All Statuses' },
              { value: 'Draft', label: 'Draft (Editable)' },
              { value: 'Applied', label: 'Applied (Immutable)' },
            ]}
            className="w-48"
          />

          {/* View mode toggle */}
          <div className="flex items-center rounded-lg border border-border bg-background p-1 gap-1">
            <button
              type="button"
              onClick={() => setViewMode('grid')}
              className={`flex items-center gap-1 px-2.5 py-1 text-xs font-semibold rounded-md transition-colors ${
                viewMode === 'grid'
                  ? 'bg-primary text-primary-foreground shadow-sm'
                  : 'text-muted-foreground hover:text-foreground'
              }`}
            >
              <LayoutGrid className="h-3.5 w-3.5" /> Grid
            </button>
            <button
              type="button"
              onClick={() => setViewMode('list')}
              className={`flex items-center gap-1 px-2.5 py-1 text-xs font-semibold rounded-md transition-colors ${
                viewMode === 'list'
                  ? 'bg-primary text-primary-foreground shadow-sm'
                  : 'text-muted-foreground hover:text-foreground'
              }`}
            >
              <List className="h-3.5 w-3.5" /> List
            </button>
          </div>
        </div>
      </div>

      {/* Main Content Area */}
      {isLoading ? (
        <div className="h-64 rounded-xl border border-border/60 bg-card p-8 flex items-center justify-center animate-pulse">
          <p className="text-sm text-muted-foreground">Loading resume revisions...</p>
        </div>
      ) : isError ? (
        <div className="rounded-xl border border-destructive/30 bg-destructive/10 p-8 text-center">
          <p className="text-sm text-destructive font-medium mb-2">Failed to load resume revisions.</p>
          <button
            type="button"
            onClick={() => refetch()}
            className="rounded-lg bg-destructive px-4 py-2 text-xs font-semibold text-destructive-foreground hover:bg-destructive/90"
          >
            Retry
          </button>
        </div>
      ) : filteredRevisions.length === 0 ? (
        <div className="rounded-xl border border-dashed border-border/80 bg-card p-12 text-center shadow-sm">
          <FileUser className="mx-auto h-12 w-12 text-muted-foreground/40 mb-3" />
          <h3 className="text-base font-semibold text-foreground">No resume revisions found</h3>
          <p className="mt-1 text-sm text-muted-foreground max-w-sm mx-auto">
            {searchQuery || statusFilter
              ? 'No resume revisions match your active search terms or status filter.'
              : 'Create your first draft resume revision snapshot to start tailoring applications.'}
          </p>
          <button
            type="button"
            onClick={handleOpenCreate}
            className="mt-4 inline-flex items-center gap-2 rounded-xl bg-primary px-4 py-2 text-xs font-semibold text-primary-foreground shadow-sm hover:bg-primary/90 transition-all"
          >
            <Plus className="h-4 w-4" /> Create Draft Resume
          </button>
        </div>
      ) : viewMode === 'grid' ? (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-5">
          {filteredRevisions.map((revision) => (
            <ResumeCard
              key={revision.id}
              revision={revision}
              onView={handleOpenDetail}
              onEdit={handleOpenEdit}
              onSpawn={handleSpawnVersion}
              isSpawning={spawnMutation.isPending && spawnMutation.variables === revision.id}
            />
          ))}
        </div>
      ) : (
        /* List / Table View */
        <div className="rounded-xl border border-border/60 bg-card shadow-sm overflow-hidden">
          <div className="overflow-x-auto">
            <table className="w-full text-left text-xs">
              <thead className="border-b border-border/60 bg-accent/40 text-muted-foreground font-semibold">
                <tr>
                  <th className="px-4 py-3">Version</th>
                  <th className="px-4 py-3">Developer / Email</th>
                  <th className="px-4 py-3">Status</th>
                  <th className="px-4 py-3">Skills</th>
                  <th className="px-4 py-3">Last Updated</th>
                  <th className="px-4 py-3 text-right">Actions</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-border/40">
                {filteredRevisions.map((rev) => (
                  <tr
                    key={rev.id}
                    className="hover:bg-accent/20 transition-colors cursor-pointer"
                    onClick={() => handleOpenDetail(rev)}
                  >
                    <td className="px-4 py-3 font-bold text-foreground">
                      <div className="flex items-center gap-1.5">
                        <span className="px-2 py-0.5 rounded bg-primary/10 text-primary font-bold">
                          v{rev.version}
                        </span>
                        {rev.parentRevisionId && (
                          <span title="Branched">
                            <GitBranch className="h-3 w-3 text-muted-foreground" />
                          </span>
                        )}
                      </div>
                    </td>
                    <td className="px-4 py-3">
                      <div className="font-bold text-foreground flex items-center gap-1.5">
                        <User className="h-3.5 w-3.5 text-primary shrink-0" />
                        {rev.personalInfo?.fullName || 'Untitled'}
                      </div>
                      <div className="text-muted-foreground text-[11px]">
                        {rev.personalInfo?.email}
                      </div>
                    </td>
                    <td className="px-4 py-3">
                      <ResumeStatusBadge status={rev.status} size="sm" />
                    </td>
                    <td className="px-4 py-3">
                      <div className="flex items-center gap-1">
                        <Award className="h-3.5 w-3.5 text-primary shrink-0" />
                        <span>{rev.skills?.length || 0} skill(s)</span>
                      </div>
                    </td>
                    <td className="px-4 py-3 text-muted-foreground">
                      {new Date(rev.updatedAt).toLocaleDateString()}
                    </td>
                    <td className="px-4 py-3 text-right">
                      <div
                        className="flex items-center justify-end gap-1.5"
                        onClick={(e) => e.stopPropagation()}
                      >
                        <button
                          type="button"
                          onClick={() => handleOpenDetail(rev)}
                          className="rounded bg-accent px-2.5 py-1 text-xs font-semibold text-foreground hover:bg-accent/80 transition-colors"
                        >
                          Details
                        </button>
                        <button
                          type="button"
                          onClick={() => handleOpenHistory(rev)}
                          className="rounded bg-accent/60 px-2 py-1 text-xs font-semibold text-muted-foreground hover:text-foreground hover:bg-accent transition-colors"
                          title="Version History Lineage"
                        >
                          <GitBranch className="h-3.5 w-3.5" />
                        </button>
                        {rev.status === 'Draft' && (
                          <button
                            type="button"
                            onClick={() => handleOpenEdit(rev)}
                            className="rounded bg-amber-500/10 text-amber-400 border border-amber-500/30 px-2.5 py-1 text-xs font-semibold hover:bg-amber-500/20 transition-colors"
                          >
                            Edit
                          </button>
                        )}
                        {rev.status === 'Applied' && (
                          <button
                            type="button"
                            disabled={
                              spawnMutation.isPending && spawnMutation.variables === rev.id
                            }
                            onClick={() => handleSpawnVersion(rev)}
                            className="rounded bg-primary/10 text-primary border border-primary/20 px-2.5 py-1 text-xs font-semibold hover:bg-primary/20 transition-colors disabled:opacity-50"
                          >
                            Spawn
                          </button>
                        )}
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* Modals */}
      <ResumeDetailModal
        isOpen={isDetailModalOpen}
        onClose={() => setIsDetailModalOpen(false)}
        revision={selectedRevision}
        onEdit={handleOpenEdit}
        onSpawn={handleSpawnVersion}
        isSpawning={
          spawnMutation.isPending &&
          selectedRevision?.id !== undefined &&
          spawnMutation.variables === selectedRevision.id
        }
      />

      <ResumeVersionHistoryModal
        isOpen={isHistoryModalOpen}
        onClose={() => setIsHistoryModalOpen(false)}
        revisions={revisions}
        selectedRevision={selectedRevision}
        onSelectRevision={(rev) => {
          setSelectedRevision(rev);
          setIsHistoryModalOpen(false);
          setIsDetailModalOpen(true);
        }}
      />
    </div>
  );
};

export default ResumesPage;
