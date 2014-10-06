package no.miles.atmiles;

import android.app.Activity;
import android.net.Uri;
import android.os.Bundle;
import android.app.Fragment;
import android.os.Looper;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.EditText;

import java.util.Timer;
import java.util.TimerTask;

public class SearchFragment extends Fragment {
    private EditText mEditTextSearch;

    private EditText getEditTextSearch() {
        if(mEditTextSearch == null) {
            mEditTextSearch = (EditText)getActivity().findViewById(R.id.editview_search);
        }
        return mEditTextSearch;
    }

    private OnSearchStringChangeListener mListener;

    public static SearchFragment newInstance() {
        SearchFragment fragment = new SearchFragment();
        Bundle args = new Bundle();
        fragment.setArguments(args);
        return fragment;
    }
    public SearchFragment() {
        // Required empty public constructor
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {

        // Inflate the layout for this fragment
        View view = inflater.inflate(R.layout.fragment_search, container, false);

        EditText search = (EditText)view.findViewById(R.id.editview_search);
        search.addTextChangedListener(new TextWatcher() {
            private Timer timer=new Timer();
            private final long DELAY = 500; // in ms

            public void afterTextChanged(Editable s) {
                //Delay call to ensure to many calls
                timer.cancel();
                timer = new Timer();
                final String searchTerm = getEditTextSearch().getText().toString();
                timer.schedule(new TimerTask() {
                    @Override
                    public void run() {
                        Looper.prepare();
                        if(mListener!=null)
                        {
                            mListener.OnSearchStringChanged(searchTerm);
                        }
                    }
                },DELAY);
            }

            public void beforeTextChanged(CharSequence s, int start, int count, int after) {
            }

            public void onTextChanged(CharSequence s, int start, int before, int count) {
            }
        });
        return view;
    }

    @Override
    public void onAttach(Activity activity) {
        super.onAttach(activity);
        try {
            mListener = (OnSearchStringChangeListener) activity;
        } catch (ClassCastException e) {
            throw new ClassCastException(activity.toString()
                    + " must implement OnFragmentInteractionListener");
        }
    }

    @Override
    public void onDetach() {
        super.onDetach();
        mListener = null;
    }
}
