import React, { useState, useRef, useEffect, useMemo } from 'react';
import type { ResumeRevisionDto } from '@/types';
import ResumeStatusBadge from './ResumeStatusBadge';
import { Search, ChevronDown, Check, FileText, Award, AlertCircle, Calendar } from 'lucide-react';

interface ResumeRevisionSelectProps {
  value: string;
  onChange: (id: string) => void;
  revisions: ResumeRevisionDto[];
  isLoading?: boolean;
  disabled?: boolean;
}

export const ResumeRevisionSelect: React.FC<ResumeRevisionSelectProps> = ({
  value,
  onChange,
  revisions,
  isLoading = false,
  disabled = false,
}) => {
  const [isOpen, setIsOpen] = useState(false);
  const [searchQuery, setSearchQuery] = useState('');
  const containerRef = useRef<HTMLDivElement>(null);

  // Close dropdown on click outside
  useEffect(() => {
    const handleClickOutside = (e: MouseEvent) => {
      if (containerRef.current && !containerRef.current.contains(e.target as Node)) {
        setIsOpen(false);
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const selectedRevision = useMemo(
    () => revisions.find((r) => r.id === value),
    [revisions, value]
  );

  const filteredRevisions = useMemo(() => {
    if (!searchQuery.trim()) return revisions;
    const query = searchQuery.toLowerCase();
    return revisions.filter((r) => {
      const name = r.personalInfo?.fullName?.toLowerCase() || '';
      const email = r.personalInfo?.email?.toLowerCase() || '';
      const summary = r.professionalSummary?.toLowerCase() || '';
      const versionStr = `v${r.version}`.toLowerCase();
      const status = r.status.toLowerCase();
      const skillMatch = r.skills?.some((s) => s.skillName?.toLowerCase().includes(query));

      return (
        name.includes(query) ||
        email.includes(query) ||
        summary.includes(query) ||
        versionStr.includes(query) ||
        status.includes(query) ||
        skillMatch
      );
    });
  }, [revisions, searchQuery]);

  return (
    <div ref={containerRef} className="relative w-full">
      {/* Selector Trigger Button */}
      <button
        type="button"
        disabled={disabled || isLoading || revisions.length === 0}
        onClick={() => !disabled && !isLoading && setIsOpen(!isOpen)}
        className={`flex w-full items-center justify-between rounded-xl border border-border bg-background px-3.5 py-2.5 text-left text-sm font-medium text-foreground hover:bg-accent/50 focus:outline-none focus:ring-2 focus:ring-primary transition-colors cursor-pointer disabled:opacity-50 disabled:cursor-not-allowed`}
      >
        <div className="flex items-center gap-2.5 overflow-hidden">
          <FileText className="h-4 w-4 text-primary shrink-0" />
          {selectedRevision ? (
            <div className="flex items-center gap-2 truncate">
              <span className="font-bold text-foreground truncate">
                {selectedRevision.personalInfo?.fullName || 'Untitled Profile'}
              </span>
              <span className="inline-flex items-center px-2 py-0.5 rounded bg-primary/10 text-primary text-xs font-bold border border-primary/20 shrink-0">
                v{selectedRevision.version}
              </span>
              <ResumeStatusBadge status={selectedRevision.status} size="sm" />
            </div>
          ) : (
            <span className="text-muted-foreground">
              {isLoading
                ? 'Loading resume revisions...'
                : revisions.length === 0
                ? 'No resume revisions available'
                : 'Select a resume revision...'}
            </span>
          )}
        </div>
        <ChevronDown
          className={`h-4 w-4 text-muted-foreground transition-transform duration-200 shrink-0 ml-2 ${
            isOpen ? 'rotate-180' : ''
          }`}
        />
      </button>

      {/* Searchable Dropdown Panel */}
      {isOpen && (
        <div className="absolute left-0 right-0 z-50 mt-1 max-h-72 w-full overflow-hidden rounded-xl border border-border bg-card shadow-xl backdrop-blur-md flex flex-col">
          {/* Search Input Header */}
          <div className="p-2 border-b border-border/60 bg-muted/30 sticky top-0 z-10">
            <div className="relative">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-3.5 w-3.5 text-muted-foreground" />
              <input
                type="text"
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                placeholder="Search by name, version, skill, or status..."
                className="w-full rounded-lg border border-border bg-background pl-8 pr-3 py-1.5 text-xs text-foreground placeholder:text-muted-foreground/60 focus:outline-none focus:ring-1 focus:ring-primary"
                autoFocus
              />
            </div>
          </div>

          {/* Options List */}
          <div className="overflow-y-auto p-1 divide-y divide-border/30 max-h-60">
            {filteredRevisions.length === 0 ? (
              <div className="px-4 py-6 text-center text-xs text-muted-foreground">
                <AlertCircle className="mx-auto h-6 w-6 text-muted-foreground/40 mb-1" />
                No matching resume revisions found
              </div>
            ) : (
              filteredRevisions.map((rev) => {
                const isSelected = rev.id === value;
                return (
                  <button
                    key={rev.id}
                    type="button"
                    onClick={() => {
                      onChange(rev.id);
                      setIsOpen(false);
                    }}
                    className={`flex w-full items-start justify-between p-2.5 text-left rounded-lg transition-colors cursor-pointer ${
                      isSelected
                        ? 'bg-primary/15 text-foreground font-medium'
                        : 'hover:bg-accent hover:text-accent-foreground'
                    }`}
                  >
                    <div className="space-y-1 min-w-0 pr-2">
                      <div className="flex items-center gap-2 flex-wrap">
                        <span className="px-1.5 py-0.5 rounded bg-primary/10 text-primary text-[11px] font-bold border border-primary/20 shrink-0">
                          v{rev.version}
                        </span>
                        <span className="text-xs font-bold text-foreground truncate">
                          {rev.personalInfo?.fullName || 'Untitled Profile'}
                        </span>
                        <ResumeStatusBadge status={rev.status} size="sm" />
                      </div>

                      {rev.professionalSummary && (
                        <p className="text-[11px] text-muted-foreground line-clamp-1 italic">
                          "{rev.professionalSummary}"
                        </p>
                      )}

                      <div className="flex items-center gap-3 text-[10px] text-muted-foreground pt-0.5">
                        {rev.skills && rev.skills.length > 0 && (
                          <span className="flex items-center gap-1 text-primary">
                            <Award className="h-3 w-3 shrink-0" />
                            {rev.skills.length} skills
                          </span>
                        )}
                        <span className="flex items-center gap-1">
                          <Calendar className="h-3 w-3 shrink-0 text-muted-foreground" />
                          Updated {new Date(rev.updatedAt).toLocaleDateString()}
                        </span>
                      </div>
                    </div>

                    {isSelected && <Check className="h-4 w-4 text-primary shrink-0 mt-0.5" />}
                  </button>
                );
              })
            )}
          </div>
        </div>
      )}

      {/* Selected Revision Details Card */}
      {selectedRevision && (
        <div className="mt-2.5 rounded-xl border border-primary/30 bg-primary/5 p-3 text-xs space-y-2">
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-2">
              <span className="font-bold text-foreground">Selected Revision:</span>
              <span className="px-2 py-0.5 rounded bg-primary/10 text-primary font-bold text-xs border border-primary/20">
                v{selectedRevision.version}
              </span>
              <ResumeStatusBadge status={selectedRevision.status} size="sm" />
            </div>
            <span className="text-[11px] text-muted-foreground">
              Updated {new Date(selectedRevision.updatedAt).toLocaleDateString()}
            </span>
          </div>

          <div className="text-muted-foreground font-medium">
            Developer: <strong className="text-foreground">{selectedRevision.personalInfo?.fullName || 'Untitled Profile'}</strong>
            {selectedRevision.personalInfo?.email && (
              <span className="ml-2 text-muted-foreground/80">({selectedRevision.personalInfo.email})</span>
            )}
          </div>

          {selectedRevision.skills && selectedRevision.skills.length > 0 && (
            <div className="flex flex-wrap gap-1 pt-1">
              {selectedRevision.skills.slice(0, 5).map((skill) => (
                <span
                  key={skill.masterSkillId}
                  className="px-2 py-0.5 rounded bg-card text-[10px] font-medium border border-border text-foreground"
                >
                  {skill.skillName}
                </span>
              ))}
              {selectedRevision.skills.length > 5 && (
                <span className="px-1.5 py-0.5 rounded bg-muted text-[10px] text-muted-foreground font-medium">
                  +{selectedRevision.skills.length - 5} more
                </span>
              )}
            </div>
          )}
        </div>
      )}
    </div>
  );
};

export default ResumeRevisionSelect;
