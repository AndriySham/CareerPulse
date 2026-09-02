import React, { useState, useRef, useEffect, useMemo } from 'react';
import type { ResumeRevisionDto } from '@/types';
import ResumeStatusBadge from './ResumeStatusBadge';
import {
  Search,
  ChevronDown,
  Check,
  FileText,
  Award,
  AlertCircle,
  Calendar,
  X,
  MapPin,
  Briefcase,
  Layers,
} from 'lucide-react';

interface ResumeRevisionSelectProps {
  value: string;
  onChange: (id: string) => void;
  revisions: ResumeRevisionDto[];
  isLoading?: boolean;
  disabled?: boolean;
  showDetailsCard?: boolean;
}

export const ResumeRevisionSelect: React.FC<ResumeRevisionSelectProps> = ({
  value,
  onChange,
  revisions,
  isLoading = false,
  disabled = false,
  showDetailsCard = true,
}) => {
  const [isOpen, setIsOpen] = useState(false);
  const [searchQuery, setSearchQuery] = useState('');
  const containerRef = useRef<HTMLDivElement>(null);
  const searchInputRef = useRef<HTMLInputElement>(null);

  // Close dropdown on click outside or Escape key
  useEffect(() => {
    const handleClickOutside = (e: MouseEvent) => {
      if (containerRef.current && !containerRef.current.contains(e.target as Node)) {
        setIsOpen(false);
      }
    };
    const handleKeyDown = (e: KeyboardEvent) => {
      if (e.key === 'Escape') {
        setIsOpen(false);
      }
    };

    if (isOpen) {
      document.addEventListener('mousedown', handleClickOutside);
      document.addEventListener('keydown', handleKeyDown);
    }

    return () => {
      document.removeEventListener('mousedown', handleClickOutside);
      document.removeEventListener('keydown', handleKeyDown);
    };
  }, [isOpen]);

  // Focus search input when opening
  useEffect(() => {
    if (isOpen) {
      setTimeout(() => {
        searchInputRef.current?.focus();
      }, 50);
    } else {
      setSearchQuery('');
    }
  }, [isOpen]);

  const selectedRevision = useMemo(
    () => revisions.find((r) => r.id === value),
    [revisions, value]
  );

  const filteredRevisions = useMemo(() => {
    if (!searchQuery.trim()) return revisions;
    const query = searchQuery.toLowerCase().trim();

    return revisions.filter((rev) => {
      const resumeName = rev.resumeName?.toLowerCase() || '';
      const targetRole = rev.targetRole?.toLowerCase() || '';
      const track = rev.track?.toLowerCase() || '';
      const careerLevel = rev.careerLevel?.toLowerCase() || '';
      const name = rev.personalInfo?.fullName?.toLowerCase() || '';
      const email = rev.personalInfo?.email?.toLowerCase() || '';
      const location = rev.personalInfo?.location?.toLowerCase() || '';
      const summary = rev.professionalSummary?.toLowerCase() || '';
      const versionStr = `v${rev.version}`.toLowerCase();
      const versionRaw = `${rev.version}`;
      const status = rev.status.toLowerCase();
      const skillMatch = rev.skills?.some((s) => s.skillName?.toLowerCase().includes(query));

      return (
        resumeName.includes(query) ||
        targetRole.includes(query) ||
        track.includes(query) ||
        careerLevel.includes(query) ||
        name.includes(query) ||
        email.includes(query) ||
        location.includes(query) ||
        summary.includes(query) ||
        versionStr.includes(query) ||
        versionRaw === query ||
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
        aria-expanded={isOpen}
        aria-haspopup="listbox"
        className="flex w-full items-center justify-between rounded-xl border border-border bg-background px-3.5 py-2.5 text-left text-sm font-medium text-foreground hover:bg-accent/50 focus:outline-none focus:ring-2 focus:ring-primary transition-colors cursor-pointer disabled:opacity-50 disabled:cursor-not-allowed"
      >
        <div className="flex items-center gap-2.5 overflow-hidden">
          <FileText className="h-4 w-4 text-primary shrink-0" />
          {selectedRevision ? (
            <div className="flex items-center gap-2 truncate">
              <span className="font-bold text-foreground truncate">
                {selectedRevision.resumeName || selectedRevision.targetRole || selectedRevision.personalInfo?.fullName || 'Resume Profile'}
              </span>
              <span className="inline-flex items-center px-2 py-0.5 rounded bg-primary/10 text-primary text-xs font-bold border border-primary/20 shrink-0">
                v{selectedRevision.version}
              </span>
              <ResumeStatusBadge status={selectedRevision.status} size="sm" />
              {selectedRevision.track && selectedRevision.careerLevel && (
                <span className="text-[11px] text-muted-foreground hidden md:inline-flex items-center gap-1 shrink-0">
                  • {selectedRevision.track} ({selectedRevision.careerLevel})
                </span>
              )}
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
        <div className="absolute left-0 right-0 z-50 mt-1 max-h-80 w-full overflow-hidden rounded-xl border border-border bg-card shadow-xl backdrop-blur-md flex flex-col animate-in fade-in-50 zoom-in-95 duration-100">
          {/* Search Input Header */}
          <div className="p-2 border-b border-border/60 bg-muted/30 sticky top-0 z-10">
            <div className="relative">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-3.5 w-3.5 text-muted-foreground" />
              <input
                ref={searchInputRef}
                type="text"
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                placeholder="Search by resume name, track, level, version, or skills..."
                className="w-full rounded-lg border border-border bg-background pl-8 pr-8 py-1.5 text-xs text-foreground placeholder:text-muted-foreground/60 focus:outline-none focus:ring-1 focus:ring-primary"
              />
              {searchQuery && (
                <button
                  type="button"
                  onClick={() => setSearchQuery('')}
                  className="absolute right-2.5 top-1/2 -translate-y-1/2 p-0.5 rounded-full text-muted-foreground hover:text-foreground cursor-pointer"
                  title="Clear search"
                >
                  <X className="h-3 w-3" />
                </button>
              )}
            </div>
          </div>

          {/* Options List */}
          <div role="listbox" className="overflow-y-auto p-1 divide-y divide-border/30 max-h-64">
            {filteredRevisions.length === 0 ? (
              <div className="px-4 py-6 text-center text-xs text-muted-foreground">
                <AlertCircle className="mx-auto h-6 w-6 text-muted-foreground/40 mb-1" />
                No matching resume revisions found
              </div>
            ) : (
              filteredRevisions.map((rev) => {
                const isSelected = rev.id === value;
                const displayName = rev.resumeName || rev.targetRole || rev.personalInfo?.fullName || 'Resume Profile';
                return (
                  <button
                    key={rev.id}
                    type="button"
                    role="option"
                    aria-selected={isSelected}
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
                      {/* Primary Header: Resume Name & Version & Status */}
                      <div className="flex items-center gap-2 flex-wrap">
                        <span className="text-xs font-bold text-foreground">
                          {displayName}
                        </span>
                        <span className="px-1.5 py-0.5 rounded bg-primary/10 text-primary text-[11px] font-bold border border-primary/20 shrink-0">
                          v{rev.version}
                        </span>
                        <ResumeStatusBadge status={rev.status} size="sm" />
                      </div>

                      {/* Secondary Context: Track • Career Level • Target Role • Location */}
                      <div className="flex items-center gap-2 text-[11px] text-muted-foreground flex-wrap">
                        {(rev.track || rev.careerLevel) && (
                          <span className="inline-flex items-center gap-1 font-medium text-foreground/80">
                            <Briefcase className="h-3 w-3 text-primary/70 shrink-0" />
                            {rev.track} • {rev.careerLevel}
                            {rev.targetRole && ` — ${rev.targetRole}`}
                          </span>
                        )}
                        {rev.personalInfo?.location && (
                          <span className="flex items-center gap-1">
                            • <MapPin className="h-3 w-3 text-muted-foreground/70 shrink-0" />
                            {rev.personalInfo.location}
                          </span>
                        )}
                      </div>

                      {/* Professional Summary Quote Snippet */}
                      {rev.professionalSummary && (
                        <p className="text-[11px] text-muted-foreground line-clamp-1 italic">
                          &quot;{rev.professionalSummary}&quot;
                        </p>
                      )}

                      {/* Top Skills Preview */}
                      {rev.skills && rev.skills.length > 0 && (
                        <div className="flex flex-wrap gap-1 pt-0.5">
                          {rev.skills.slice(0, 4).map((s) => (
                            <span
                              key={s.masterSkillId}
                              className="px-1.5 py-0.2 rounded bg-muted/70 text-[10px] text-muted-foreground border border-border/50"
                            >
                              {s.skillName}
                            </span>
                          ))}
                          {rev.skills.length > 4 && (
                            <span className="text-[10px] text-muted-foreground/70">
                              +{rev.skills.length - 4} more
                            </span>
                          )}
                        </div>
                      )}

                      {/* Footer: Skills total count + Updated Date */}
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
      {showDetailsCard && selectedRevision && (
        <div className="mt-2.5 rounded-xl border border-primary/30 bg-primary/5 p-3.5 text-xs space-y-2.5 animate-in fade-in duration-200">
          <div className="flex items-center justify-between flex-wrap gap-2">
            <div className="flex items-center gap-2">
              <span className="font-bold text-foreground">
                {selectedRevision.resumeName || selectedRevision.targetRole || selectedRevision.personalInfo?.fullName || 'Resume Profile'}
              </span>
              <span className="px-2 py-0.5 rounded bg-primary/10 text-primary font-bold text-xs border border-primary/20">
                v{selectedRevision.version}
              </span>
              <ResumeStatusBadge status={selectedRevision.status} size="sm" />
            </div>
            <span className="text-[11px] text-muted-foreground">
              Updated {new Date(selectedRevision.updatedAt).toLocaleDateString()}
            </span>
          </div>

          <div className="flex flex-wrap items-center gap-x-3 gap-y-1 text-xs text-muted-foreground font-medium">
            {selectedRevision.track && (
              <span className="inline-flex items-center gap-1">
                <Briefcase className="h-3.5 w-3.5 text-primary" /> Track: <strong className="text-foreground">{selectedRevision.track}</strong>
              </span>
            )}
            {selectedRevision.careerLevel && (
              <>
                <span>•</span>
                <span className="inline-flex items-center gap-1">
                  <Layers className="h-3.5 w-3.5 text-primary" /> Level: <strong className="text-foreground">{selectedRevision.careerLevel}</strong>
                </span>
              </>
            )}
            {selectedRevision.targetRole && (
              <>
                <span>•</span>
                <span className="inline-flex items-center gap-1">
                  Role: <strong className="text-foreground">{selectedRevision.targetRole}</strong>
                </span>
              </>
            )}
            {selectedRevision.personalInfo?.email && (
              <>
                <span>•</span>
                <span className="text-muted-foreground">{selectedRevision.personalInfo.email}</span>
              </>
            )}
          </div>

          {selectedRevision.professionalSummary && (
            <p className="text-[11px] text-muted-foreground line-clamp-2 leading-relaxed bg-background/50 p-2 rounded-lg border border-border/40">
              {selectedRevision.professionalSummary}
            </p>
          )}

          {selectedRevision.skills && selectedRevision.skills.length > 0 && (
            <div className="flex flex-wrap gap-1 pt-0.5">
              {selectedRevision.skills.slice(0, 6).map((skill) => (
                <span
                  key={skill.masterSkillId}
                  className="px-2 py-0.5 rounded bg-card text-[10px] font-medium border border-border text-foreground"
                >
                  {skill.skillName}
                </span>
              ))}
              {selectedRevision.skills.length > 6 && (
                <span className="px-1.5 py-0.5 rounded bg-muted text-[10px] text-muted-foreground font-medium">
                  +{selectedRevision.skills.length - 6} more
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
