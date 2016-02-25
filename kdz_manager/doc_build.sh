#!/usr/bin/env bash
pwd && doxygen Doxyfile_paper
(cd latex_paper/ && make )
(cd latex_paper/ && open refman.pdf)
